using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using csDelaunay;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace DefaultNamespace
{
    public class PlateUtils
    {
        private Voronoi voronoi;

        private static PlateUtils _plateUtils;

        private PlateUtils()
        {
        }

        public static PlateUtils getPlateUtils()
        {
            if (_plateUtils == null)
            {
                _plateUtils = new PlateUtils();
            }

            return _plateUtils;
        }

        public List<Vector2> generateCenters(int seed, int mapHeight, int mapWidth, int jitter)
        {
            List<Vector2> centers = new List<Vector2>();
            Random rand = new Random(seed);
            double mapHeightBorder = mapHeight * 0.02;
            double mapWidthBorder = mapWidth * 0.02;

            int xStart = (int)mapHeightBorder;
            int yStart = (int)mapWidthBorder;
            int xJitter = 0, yJitter = 0;
            while (yStart < mapHeight)
            {
                if (jitter >= 1)
                {
                    xJitter = rand.Next(-jitter, jitter);
                    yJitter = rand.Next(-jitter, jitter);
                }

                int posX = xStart + xJitter;
                int posY = yStart + yJitter;
                if (posX >= mapWidth)
                {
                    posX = mapWidth - 1;
                }

                if (posY >= mapHeight)
                {
                    posY = mapHeight - 1;
                }

                if (posX < 0)
                {
                    posX = 1;
                }

                if (posY < 0)
                {
                    posY = 1;
                }

                Vector2 center = new Vector2(posX, posY);

                centers.Add(center);
                xStart += 10;

                if (xStart >= mapWidth)
                {
                    xStart -= mapWidth;
                    yStart += 10;
                }
            }

            return centers;
        }

        public Voronoi getVoronoiRegions(List<Vector2> centers, int mapWidth, int mapHeight, int seed)
        {
             voronoi?.Dispose();
            Rectf bounds = new Rectf(0, 0, mapWidth, mapHeight);
            voronoi = new Voronoi(centers, bounds, 15, seed);
            return voronoi;
        }

        public List<Vector2> getNeighboursForSite(Vector2 center)
        {
            List<Edge> edgesForCenter = voronoi.SitesIndexedByLocation[center].Edges;
            List<Vector2> initialNeighborsForCenter = voronoi.NeighborSitesForSite(center);
            List<Vector2> finalNeighbors = new List<Vector2>();
            foreach (var neighbor in initialNeighborsForCenter)
            {
                List<Edge> edgesForNeighbor = voronoi.SitesIndexedByLocation[neighbor].Edges;
                foreach (var edge in edgesForNeighbor)
                {
                    if (edge.ClippedEnds == null)
                        continue;
                    if (edgesForCenter.Contains(edge) && Vector2.Distance(edge.RightSite.Coord,edge.LeftSite.Coord) > 3)
                    {
                        finalNeighbors.Add(neighbor);
                        break;
                    }
                }
            }

            return finalNeighbors;
        }

        public List<Vector2> getCentersForPlates(int numberOfPlates, int minDistance, int seed, List<Vector2> centers)
        {
            PoissonDiscSampling poissonDiscSampling = new PoissonDiscSampling(minDistance, seed, centers);
            return poissonDiscSampling.generatePoints(numberOfPlates);
        }

        public List<Vector2> getPointsInsideRegion(List<Vector2> region)
        {
            if (region.Count == 0)
                return new List<Vector2>();
            int minY = (int)region.Min(p => p.Y);
            int maxY = (int)region.Max(p => p.Y);

            List<Vector2> pointsInsideRegion = new List<Vector2>();

            for (int y = minY; y <= maxY; y++)
            {
                List<int> intersections = new List<int>();

                for (int i = 0; i < region.Count; i++)
                {
                    Vector2 v1 = region[i];
                    Vector2 v2 = region[(i + 1) % region.Count];

                    int x1 = (int)v1.X;
                    int x2 = (int)v2.X;
                    int y1 = (int)v1.Y;
                    int y2 = (int)v2.Y;

                    if (y1 <= y && y2 > y || y2 <= y && y1 > y)
                    {
                        double intersectX = (double)(x2 - x1) * (y - y1) / (y2 - y1) + x1;
                        intersections.Add((int)Math.Round(intersectX));
                    }
                }

                intersections.Sort();
                for (int i = 0; i < intersections.Count - 1; i += 2)
                {
                    for (int x = intersections[i]; x <= intersections[i + 1]; x++)
                    {
                        pointsInsideRegion.Add(new Vector2(x, y));
                    }
                }
            }

            return pointsInsideRegion;
        }

        public (List<Vector2>,List<Vector2>) getVoronoiEdgesForPlate(Plate plate)
        {
            HashSet<Vector2> points = new HashSet<Vector2>(plate.getAllPoints());
            List<Vector2> voronoiOnEdges = new List<Vector2>();
            List<Vector2> voronoiOnInnerEdge = new List<Vector2>();
            foreach (var point in points)
            {
                bool isInsideRegion = getNeighboursForSite(point).All(element => points.Contains(element));
                if (!isInsideRegion)
                {
                    voronoiOnEdges.Add(point);
                }
            }

            foreach (var voronoiEdge in voronoiOnEdges)
            {
                List<Vector2> neighbors = getNeighboursForSite(voronoiEdge);
                foreach (var neighbor in neighbors)
                {
                    if (points.Contains(neighbor) && !voronoiOnEdges.Contains(neighbor) && !voronoiOnInnerEdge.Contains(neighbor))
                    {
                        voronoiOnInnerEdge.Add(neighbor);
                    }
                }
            }
            return (voronoiOnEdges,voronoiOnInnerEdge);
        }

        public List<Plate> getNeighboursForPlate(List<Plate> plates,Plate plate)
        {
            List<Plate> neighborPlates = new List<Plate>();
            HashSet<Vector2> edgesForPlate = new HashSet<Vector2>(plate.getEdgeRing());
            foreach (var neighborPlate in plates)
            {
                if(plate.Equals(neighborPlate))
                    continue;
                foreach (var edge in neighborPlate.getEdgeRing())
                {
                    List<Vector2> neighborRegions = getNeighboursForSite(edge);
                    if (edgesForPlate.Any(element => neighborRegions.Contains(element)))
                    {
                        neighborPlates.Add(neighborPlate);
                        break;
                    }
                }
            }

            return neighborPlates;
        }
    }
}