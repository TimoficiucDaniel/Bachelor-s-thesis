using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class EvaluationUtils
    {
        public static float getErosionScore(float[,] heightMap, int mapChunkSize)
        {
            float[][] slopeMap = new float[mapChunkSize][];
            for (int index = 0; index < mapChunkSize; index++)
            {
                slopeMap[index] = new float[mapChunkSize];
            }

            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    float topNeighbor;
                    float leftNeighbor;
                    float bottomNeighbor;
                    float rightNeighbor;
                    if (x - 1 < 0)
                        leftNeighbor = heightMap[x, y];
                    else
                        leftNeighbor = heightMap[x - 1, y];
                    if (x + 1 > 0)
                        rightNeighbor = heightMap[x, y];
                    else
                        rightNeighbor = heightMap[x + 1, y];
                    if (y - 1 < 0)
                        topNeighbor = heightMap[x, y];
                    else
                        topNeighbor = heightMap[x, y - 1];
                    if (y + 1 > 0)
                        bottomNeighbor = heightMap[x, y];
                    else
                        bottomNeighbor = heightMap[x, y + 1];
                    slopeMap[x][y] = Mathf.Max(Mathf.Abs(heightMap[x, y] - topNeighbor),
                        Mathf.Abs(heightMap[x, y] - leftNeighbor),
                        Mathf.Abs(heightMap[x, y] - rightNeighbor),
                        Mathf.Abs(heightMap[x, y] - bottomNeighbor));
                }
            }

            float slopeMeanValue = 0;
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    slopeMeanValue += slopeMap[x][y];
                }
            }

            slopeMeanValue *= 1 / (float)Math.Pow(mapChunkSize, 2);

            float standardDeviation = 0;
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    standardDeviation += (float) Math.Pow(slopeMap[x][y] - slopeMeanValue, 2);
                }
            }
            standardDeviation *= 1 / (float)Math.Pow(mapChunkSize, 2);
            standardDeviation = (float) Math.Sqrt(standardDeviation);

            if (slopeMeanValue == 0)
                return 0f;
            float erosionScore = standardDeviation / slopeMeanValue;

            return erosionScore;
        }
    }
}