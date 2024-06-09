using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using csDelaunay;
using UnityEngine;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

namespace DefaultNamespace
{
    public class Arguments
    {
        public Vector2 arg1 { get; }
        public int arg2 { get; }

        public Arguments(Vector2 arg1, int arg2)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
    }

    public class PlateSimulator
    {
        private float[,] noiseMap;
        private Random random;
        private Voronoi voronoi;
        private PlateUtils plateUtils;

        private Dictionary<Vector2, object> lockDictionary;
        private Dictionary<Vector2, bool> regionsStatus;
        private List<Plate> plates;
        private List<PlateBuilder> plateBuilders;
        private int mapWidth;
        private int mapHeight;

        public List<Plate> getPlates()
        {
            return plates;
        }

        public PlateSimulator(Voronoi voronoi, float[,] noiseMap, int seed, int mapHeight, int mapWidth)
        {
            this.voronoi = voronoi;
            this.noiseMap = noiseMap;
            random = new Random(seed);
            plateUtils = PlateUtils.getPlateUtils();
            plateBuilders = new List<PlateBuilder>();
            plates = new List<Plate>();
            regionsStatus = new Dictionary<Vector2, bool>();
            lockDictionary = new Dictionary<Vector2, object>();
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
        }

        public List<Plate> initializePlates(List<Vector2> plateOrigins)
        {
            List<Vector2> allCenters = voronoi.SitesIndexedByLocation.Keys.ToList();
            foreach (var center in allCenters)
            {
                lockDictionary.Add(center, new object());
                regionsStatus.Add(center, false);
            }

            int plateIndex = 0;
            CountdownEvent countdownEvent = new CountdownEvent(plateOrigins.Count);
            foreach (var origin in plateOrigins)
            {
                PlateBuilder plateBuilder = new PlateBuilder();
                plateBuilder.setCenter(origin)
                    .setId(plateIndex)
                    .addRegion(origin);
                plateBuilders.Add(plateBuilder);
                Arguments args = new Arguments(origin, plateIndex);
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    randomFillPlates(args);
                    countdownEvent.Signal();
                });
                plateIndex++;
            }

            countdownEvent.Wait();
            foreach (var plateBuilder in plateBuilders)
            {
                plates.Add(plateBuilder.build());
            }
            return plates;
        }

        private void randomFillPlates(object state)
        {
            Arguments args = (Arguments)state;
            Vector2 origin = args.arg1;
            int plateIndex = args.arg2;
            ListWithSwapping<Vector2> workQueue = new ListWithSwapping<Vector2>();
            List<Vector2> neighborRegions = plateUtils.getNeighboursForSite(origin);
            foreach (var region in neighborRegions)
            {
                workQueue.Add(region);
            }

            QueueIterator<Vector2> iterator = workQueue.getIterator();
            
            while (iterator.hasMore())
            {
                Vector2 currentRegion = iterator.getNext();
                bool foundRegion = false;
                lock (lockDictionary[currentRegion])
                {
                    if (regionsStatus[currentRegion] == false)
                    {
                        regionsStatus[currentRegion] = true;
                        foundRegion = true;
                    }
                }

                if (!foundRegion)
                {
                    continue;
                }
               
                plateBuilders[plateIndex].addRegion(currentRegion);
                foreach (var region in plateUtils.getNeighboursForSite(currentRegion))
                {
                    workQueue.Add(region);
                }
                
                workQueue.SwapTwoElements(random.Next(workQueue.Count()), random.Next(workQueue.Count()));
            }
        }

        private void randomFillPlatesStartRecursive(object state)
        {
            Arguments args = (Arguments)state;
            Vector2 origin = args.arg1;
            int plateIndex = args.arg2;
            List<Vector2> neighborSites = plateUtils.getNeighboursForSite(origin);
            CountdownEvent countdownEvent = new CountdownEvent(neighborSites.Count);

            for (int i = 0; i < neighborSites.Count; i++)
            {
                bool plateFound = false;
                lock (lockDictionary[neighborSites[i]])
                {
                    if (regionsStatus[neighborSites[i]] == false)
                    {
                        plateFound = true;
                        regionsStatus[neighborSites[i]] = true;
                    }
                    else
                    {
                        countdownEvent.Signal();
                    }
                }

                if (plateFound)
                {
                    Arguments newArgs = new Arguments(neighborSites[i], plateIndex);
                    plates.Find(plate => plate.getId() == plateIndex).addRegion(neighborSites[i]);
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        randomFillPlates(newArgs);
                        countdownEvent.Signal();
                    });
                }
            }

            countdownEvent.Wait();
        }

        private void randomFillPlatesRecursive(object state)
        {
            Arguments args = (Arguments)state;
            Vector2 origin = args.arg1;
            int plateIndex = args.arg2;
            List<Vector2> neighborSites = plateUtils.getNeighboursForSite(origin);
            List<int> randomOrder = ShuffleExecutionOrder(Enumerable.Range(0, neighborSites.Count - 1).ToList());
            for (int i = 0; i < randomOrder.Count; i++)
            {
                bool plateFound = false;
                lock (lockDictionary[neighborSites[randomOrder[i]]])
                {
                    if (regionsStatus[neighborSites[randomOrder[i]]] == false)
                    {
                        plateFound = true;
                        regionsStatus[neighborSites[randomOrder[i]]] = true;
                    }
                }

                if (plateFound)
                {
                    Arguments newArgs = new Arguments(neighborSites[randomOrder[i]], plateIndex);
                    plates.Find(plate => plate.getId() == plateIndex).addRegion(neighborSites[randomOrder[i]]);
                    randomFillPlates(newArgs);
                }
            }
        }

        private List<int> ShuffleExecutionOrder(List<int> plateIndexes)
        {
            for (int i = plateIndexes.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1); 

                (plateIndexes[i], plateIndexes[j]) = (plateIndexes[j], plateIndexes[i]);
            }

            return plateIndexes;
        }

        public float[,] simulatePlates(int numberOfEpochs, List<Vector2> plateOrigins)
        {
            initializePlates(plateOrigins);
            assignParameters();
            for (int i = 1; i <= numberOfEpochs; i++)
            {
                foreach (var plate in plates)
                {
                    foreach (var edge in plate.getEdgeRing())
                    {
                        Vector2 displacement = edge - plate.getOrigin();
                        Vector2 displacementNormalized = normalizeVector2(displacement);
                        float angleToCenter = Vector2.Dot(displacementNormalized, plate.getDirection());
                        foreach (var point in plate.AllPointsByRegions[edge])
                        {
                            float valueToAdd = plate.getForce() * (random.Next(90, 110) / 100f) * angleToCenter;
                            try
                            {
                                if (valueToAdd < 0) valueToAdd *= 0.35f;
                                noiseMap[(int)point.X, (int)point.Y] += valueToAdd;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Debug.Log(point.X + " " + point.Y);
                            }
                        }
                    }
                    foreach (var edge in plate.getInnerEdgeRing())
                    {
                        Vector2 displacement = edge - plate.getOrigin();
                        Vector2 displacementNormalized = normalizeVector2(displacement);
                        float angleToCenter = Vector2.Dot(displacementNormalized, plate.getDirection());
                        foreach (var point in plate.AllPointsByRegions[edge])
                        {
                            float valueToAdd = plate.getForce() * (random.Next(45, 55) / 100f) * angleToCenter;
                            noiseMap[(int)point.X,(int)point.Y] += valueToAdd;
                        }
                    }
                }
            }

            normalizePoints();
            return noiseMap;
        }

        private void normalizePoints()
        {
            float maxNoiseHeight = 0f;
            float minNoiseHeight = 0f;
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (noiseMap[x, y] > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseMap[x, y];
                    }
                }
            }
            
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }
            
        }

        private Vector2 normalizeVector2(Vector2 vector2)
        {
            return new Vector2(vector2.X / vector2.Length(), vector2.Y / vector2.Length());
        }

        public void assignParameters()
        {
            foreach (var plate in plates)
            {
                plate.setDirection(normalizeVector2(new Vector2(mapWidth/2,mapHeight/2) - plate.getOrigin()));
                plate.setForce((float)random.Next(7,10)/1000f);
                (List<Vector2>, List<Vector2>) edges = plateUtils.getVoronoiEdgesForPlate(plate);
                plate.setEdgeRing(edges.Item1);
                plate.setInnerEdgeRing(edges.Item2);
                plate.setNeighbors(plateUtils.getNeighboursForPlate(plates, plate));
                foreach (var edge in edges.Item1)
                {
                    plate.AllPointsByRegions.Add(edge,plateUtils.getPointsInsideRegion(voronoi.Region(edge)));
                }
                foreach (var edge in edges.Item2)
                {
                    plate.AllPointsByRegions.Add(edge,plateUtils.getPointsInsideRegion(voronoi.Region(edge)));
                }
            }
        }
    }
}