using System;
using System.Collections.Generic;
using System.Linq;
using csDelaunay;
using DefaultNamespace;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        Voronoi,
        Plates,
        FinalMap
    };

    public DrawMode drawMode;
    private float[,] noiseMap;
    private float[,] initialNoiseMap;
    public const int mapChunkSize = 1024;
    public float scale;

    public int octaves;
    [Range(0,1)]
    public float persistence;
    public float lacunarity;

    public int seed;

    public bool autoUpdate;
    public string saveLocation;

    public int numberOfEpochsPlates = 50;
    public int numberOfPlates = 5;

    public int iterationsThermal = 100;
    public float talusAngle = 0.005f;
    public float erosionFactor = 0.25f;

    private List<Plate> plates;
    private Voronoi voronoi;

    public void generateMap()
    {
        PlateUtils plateUtils = PlateUtils.getPlateUtils();
        List<Vector2> centers = plateUtils.generateCenters(seed, mapChunkSize, mapChunkSize, 5);
        voronoi = plateUtils.getVoronoiRegions(centers,mapChunkSize,mapChunkSize, seed);   
        noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, scale, octaves, persistence, lacunarity);
        initialNoiseMap = (float[,])noiseMap.Clone();
        PlateSimulator plateSimulator = new PlateSimulator(voronoi, noiseMap, seed, mapChunkSize, mapChunkSize);
        noiseMap = plateSimulator.simulatePlates(numberOfEpochsPlates, plateUtils.getCentersForPlates(
            numberOfPlates, 40, seed, voronoi.SitesIndexedByLocation.Keys.ToList()));
        plates = plateSimulator.getPlates();
        Debug.Log("Erosion score is : "+ EvaluationUtils.getErosionScore(noiseMap,mapChunkSize));
        noiseMap = ThermalErosion.applyThermalErosion(noiseMap, mapChunkSize, mapChunkSize, iterationsThermal, talusAngle, erosionFactor);
        Debug.Log("Erosion score is : "+ EvaluationUtils.getErosionScore(noiseMap,mapChunkSize));
        noiseMap = FalloffGenerator.applyFalloff(mapChunkSize, noiseMap);

        NoiseMapToPng.convertNoiseMapToPng(TextureGenerator.TextureFromHeightMap(noiseMap), saveLocation);
        DrawMap();
    }

    public void DrawMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(initialNoiseMap), mapChunkSize, mapChunkSize);
        }
        else if (drawMode == DrawMode.Voronoi)
        {
            display.DisplayVoronoiDiagram(TextureGenerator.TextureFromHeightMap(noiseMap), voronoi, mapChunkSize,
                mapChunkSize);
        }
        else if (drawMode == DrawMode.Plates)
        {
            display.DisplayVoronoiDiagramTest(TextureGenerator.TextureFromHeightMap(noiseMap), voronoi, plates, mapChunkSize,
                mapChunkSize);
        }
        else if (drawMode == DrawMode.FinalMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap), mapChunkSize, mapChunkSize);
        }
    }
    
}