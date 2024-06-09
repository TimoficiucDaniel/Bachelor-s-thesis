﻿using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

    public static float[,] applyFalloff(int size, float[,] noiseMap)
    {
        float[,] falloffMap = GenerateFalloffMap(size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                noiseMap[i, j] -= falloffMap[i, j];
            }
        }
        return noiseMap;
    }

    public static float[,] GenerateFalloffMap(int size) {
        float[,] map = new float[size,size];

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max (Mathf.Abs (x), Mathf.Abs (y));
                map [i, j] = Evaluate(value);
            }
        }

        return map;
    }

    private static float Evaluate(float value) {
        float a = 2f;
        float b = 5f;

        return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow (b - b * value, a));
    }
}