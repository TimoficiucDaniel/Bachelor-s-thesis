using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

namespace DefaultNamespace
{
    public class PoissonDiscSampling
    {
        private float minDist;
        private const int maxAttempts = 30;

        private readonly Random random;

        private List<Vector2> points;
        private List<Vector2> voronoiCenters;

        public PoissonDiscSampling(int minDist, int seed, List<Vector2> voronoiCenters)
        {
            this.minDist = minDist;
            random = new Random(seed);
            this.voronoiCenters = voronoiCenters;
            points = new List<Vector2>();
        }

        public List<Vector2> generatePoints(int numPoints)
        {
            Vector2 initialPoint = voronoiCenters[random.Next(voronoiCenters.Count)];
            points.Add(initialPoint);
            while (points.Count < numPoints)
            {
                bool foundValidPoint = false;

                for (int i = 0; i < maxAttempts; i++)
                {
                    Vector2 basePoint = points[random.Next(points.Count)];
                    Vector2 newPoint = generateRandomPoint();

                    if (isValid(newPoint))
                    {
                        points.Add(newPoint);
                        foundValidPoint = true;
                        break;
                    }
                }

                if (!foundValidPoint)
                {
                    int indexToRemove = random.Next(points.Count);
                    points.RemoveAt(indexToRemove);
                }
            }
            return points;
        }

        private Vector2 generateRandomPoint()
        {
            Vector2 newPoint = voronoiCenters[random.Next(voronoiCenters.Count)];
            return newPoint;
        }

        private bool isValid(Vector2 newPoint)
        {
            foreach (var point in points)
            {
                if (Vector2.Distance(point, newPoint) < minDist)
                    return false;
            }

            return true;
        }
        
    }
}