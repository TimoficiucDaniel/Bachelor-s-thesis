using UnityEngine;

namespace DefaultNamespace
{
    public class ThermalErosion
    {
        public static float[,] applyThermalErosion(float[,] noiseMap, int width, int height, int iterations = 100, float talusAngle = 0.005f,
            float erosionFactor = 0.25f)
        {
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    for (int y = 1; y < height - 1; y++)
                    {
                        float currentHeight = noiseMap[x, y];

                        // Get the heights of the 4 neighboring points
                        float heightLeft = noiseMap[x - 1, y];
                        float heightRight = noiseMap[x + 1, y];
                        float heightDown = noiseMap[x, y - 1];
                        float heightUp = noiseMap[x, y + 1];

                        // Calculate the differences in height
                        float diffLeft = currentHeight - heightLeft;
                        float diffRight = currentHeight - heightRight;
                        float diffDown = currentHeight - heightDown;
                        float diffUp = currentHeight - heightUp;

                        // Apply erosion based on the height differences
                        if (diffLeft > talusAngle)
                        {
                            float moveAmount = erosionFactor * (diffLeft - talusAngle);
                            noiseMap[x, y] -= moveAmount;
                            noiseMap[x - 1, y] += moveAmount;
                        }

                        if (diffRight > talusAngle)
                        {
                            float moveAmount = erosionFactor * (diffRight - talusAngle);
                            noiseMap[x, y] -= moveAmount;
                            noiseMap[x + 1, y] += moveAmount;
                        }

                        if (diffDown > talusAngle)
                        {
                            float moveAmount = erosionFactor * (diffDown - talusAngle);
                            noiseMap[x, y] -= moveAmount;
                            noiseMap[x, y - 1] += moveAmount;
                        }

                        if (diffUp > talusAngle)
                        {
                            float moveAmount = erosionFactor * (diffUp - talusAngle);
                            noiseMap[x, y] -= moveAmount;
                            noiseMap[x, y + 1] += moveAmount;
                        }
                    }
                }
            }

            noiseMap = normalizeHeightMap(width, height, noiseMap);
            return noiseMap;
        }

        public static float[,] normalizeHeightMap(int width, int height, float[,] noiseMap)
        {
            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    float noiseHeight = noiseMap[x, y];
                    if(noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if(noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                }
            }
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}