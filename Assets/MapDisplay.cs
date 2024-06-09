using System;
using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;

    public void DrawNoiseMap(Texture2D texture, int width, int height)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(width, 1, height);
    }

    public void DisplayVoronoiDiagram(Texture2D tx, Voronoi voronoi, int width, int height)
    {
        Dictionary<Vector2, Site> voronoiRegions = voronoi.SitesIndexedByLocation;
        List<Edge> edges = voronoi.Edges;
        foreach (KeyValuePair<Vector2, Site> kv in voronoiRegions)
        {
            tx.SetPixel((int)kv.Key.X, (int)kv.Key.Y, Color.red);
        }
        
        foreach (Edge edge in edges)
        {
            if (edge.ClippedEnds == null)
            {
                continue;
            }

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        textureRender.material.mainTexture = tx;
        textureRender.transform.localScale = new Vector3(width, 1, height);
    }

    public void DisplayVoronoiDiagramTest(Texture2D tx, Voronoi voronoi, List<Plate> plates, int width, int height)
    {
        PlateUtils plateUtils = PlateUtils.getPlateUtils();
        foreach (var plate in plates)
        {
            List<Vector2> allCenters = plate.getAllPoints();
            Color color = new Color(Random.value, Random.value, Random.value);
            foreach (var point in allCenters)
            {
                List<Vector2> allpoints = plateUtils.getPointsInsideRegion(voronoi.Region(point));
                foreach (var pixel in allpoints)
                {
                    tx.SetPixel((int)pixel.X, (int)pixel.Y, color);
                }
            }
        }
        tx.Apply();

        textureRender.material.mainTexture = tx;
        textureRender.transform.localScale = new Vector3(width, 1, height);
    }

    public void DrawTexture(Texture2D texture, int width, int height)
    {
        texture.Apply();
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(width, 1, height);
    }

    private void DrawLine(Vector2 p0, Vector2 p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.X;
        int y0 = (int)p0.Y;
        int x1 = (int)p1.X;
        int y1 = (int)p1.Y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    
}