using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace
{
    public class World : MonoBehaviour
    {
        public int mapSizeInChunks = 6;
        public int chunkSize = 16, chunkHeight = 150;
        public int waterThreshold = 50;
        public GameObject chunkPrefab;
        public string mapLocation;
        public float[,] noiseMap;
        public GameObject canvas;
        public GameObject plane;
        
        private Dictionary<Vector3, ChunkData> chunkDataDictionary = new Dictionary<Vector3, ChunkData>();
        private Dictionary<Vector3, ChunkRenderer> chunkDictionary = new Dictionary<Vector3, ChunkRenderer>();


        public void Start()
        {
            canvas.SetActive(false);
            plane.SetActive(false);
            GenerateWorld();
        }
        
        public void GenerateWorld()
        {
            canvas.SetActive(false);
            noiseMap = NoiseMapToPng.loadNoiseMapFromPNG(mapLocation);
            chunkDataDictionary.Clear();
            foreach (ChunkRenderer chunk in chunkDictionary.Values)
            {
                Destroy(chunk.gameObject);
            }

            chunkDictionary.Clear();

            for (int x = 0; x < mapSizeInChunks; x++)
            {
                for (int z = 0; z < mapSizeInChunks; z++)
                {
                    ChunkData data = new ChunkData(chunkSize, chunkHeight, this,
                        new Vector3(x * chunkSize, 0, z * chunkSize));
                    GenerateVoxels(data);
                    chunkDataDictionary.Add(data.worldPosition, data);
                }
            }

            foreach (ChunkData data in chunkDataDictionary.Values)
            {
                MeshData meshData = Chunk.GetChunkMeshData(data);
                GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
                ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
                chunkDictionary.Add(data.worldPosition, chunkRenderer);
                chunkRenderer.InitializeChunk(data);
                chunkRenderer.UpdateChunk(meshData);
            }
        }

        private void GenerateVoxels(ChunkData data)
        {
            for (int x = 0; x < data.chunkSize; x++)
            {
                for (int z = 0; z < data.chunkSize; z++)
                {
                    float noiseValue = noiseMap[Mathf.FloorToInt(x + data.worldPosition.x), Mathf.FloorToInt(z + data.worldPosition.z)];
                    int groundPosition = Mathf.RoundToInt(noiseValue*chunkHeight);
                    for (int y = 0; y < chunkHeight; y++)
                    {
                        VoxelType voxelType = VoxelType.Dirt;
                        if (y > groundPosition)
                        {
                            if (y < waterThreshold)
                            {
                                voxelType = VoxelType.Water;
                            }
                            else
                            {
                                voxelType = VoxelType.Air;
                            }
                        }
                        else if (y == groundPosition)
                        {
                            if (groundPosition >= 0.65f * chunkHeight)
                                voxelType = VoxelType.Stone;
                            else if (groundPosition <= waterThreshold * 1.1f)
                                voxelType = VoxelType.Sand;
                            else
                                voxelType = VoxelType.Grass_Dirt;
                        }

                        Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                    }
                }
            }
        }

        internal VoxelType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
        {
            Vector3 pos = Chunk.ChunkPositionFromVoxelCoordinates(this, x, y, z);

            chunkDataDictionary.TryGetValue(pos, out var containerChunk);

            if (containerChunk == null)
                return VoxelType.Nothing;
            Vector3 blockInChunkCoordinates = Chunk.GetVoxelInChunkCoordinates(containerChunk, new Vector3(x, y, z));
            return Chunk.GetVoxelFromChunkCoordinates(containerChunk, blockInChunkCoordinates);
        }
    }
}