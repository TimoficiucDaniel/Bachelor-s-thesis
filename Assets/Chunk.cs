using System;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Chunk
    {
        public static MeshData GetChunkMeshData(ChunkData chunkData)
        {
            MeshData meshData = new MeshData(true);

            LoopThroughTheBlocks(chunkData,
                (x, y, z) => meshData = VoxelHelper.GetMeshData(chunkData, x, y, z, meshData,
                    chunkData.voxels[GetIndexFromPosition(chunkData, x, y, z)]));
            
            return meshData;
        }

        public static void LoopThroughTheBlocks(ChunkData chunkData, Action<int, int, int> actionToPerform)
        {
            for (int index = 0; index < chunkData.voxels.Length; index++)
            {
                var position = GetPositionFromIndex(chunkData, index);
                actionToPerform((int)position.x, (int)position.y, (int)position.z);
            }
        }

        private static Vector3 GetPositionFromIndex(ChunkData chunkData, int index)
        {
            int x = index % chunkData.chunkSize;
            int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
            int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
            return new Vector3(x, y, z);
        }

        private static bool InRange(ChunkData chunkData, int axisCoordinate)
        {
            if (axisCoordinate < 0 || axisCoordinate >= chunkData.chunkSize)
            {
                return false;
            }

            return true;
        }

        private static bool InRangeHeight(ChunkData chunkData, int yCoordinate)
        {
            if (yCoordinate < 0 || yCoordinate >= chunkData.chunkHeight)
            {
                return false;
            }

            return true;
        }

        public static VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
        {
            if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z))
            {
                int index = GetIndexFromPosition(chunkData, x, y, z);
                return chunkData.voxels[index];
            }

            return chunkData.worldReference.GetBlockFromChunkCoordinates(chunkData, (int)chunkData.worldPosition.x + x,
                (int)chunkData.worldPosition.y + y, (int)chunkData.worldPosition.z + z);
        }

        public static void SetBlock(ChunkData chunkData, Vector3 localposition, VoxelType voxel)
        {
            if (InRange(chunkData, (int)localposition.x) && InRangeHeight(chunkData, (int)localposition.y) &&
                InRange(chunkData, (int)localposition.z))
            {
                int index = GetIndexFromPosition(chunkData, (int)localposition.x, (int)localposition.y,
                    (int)localposition.z);
                chunkData.voxels[index] = voxel;
            }
            else
            {
                throw new Exception("Need to ask for appropriate chunk");
            }
        }

        private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
        {
            return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
        }

        public static Vector3 GetVoxelInChunkCoordinates(ChunkData chunkData, Vector3 pos)
        {
            return new Vector3
            {
                x = pos.x - chunkData.worldPosition.x,
                y = pos.y - chunkData.worldPosition.y,
                z = pos.z - chunkData.worldPosition.z
            };
        }

        public static VoxelType GetVoxelFromChunkCoordinates(ChunkData chunkData, Vector3 chunkCoordinates)
        {
            return GetVoxelFromChunkCoordinates(chunkData, (int)chunkCoordinates.x, (int)chunkCoordinates.y,
                (int)chunkCoordinates.z);
        }

        public static Vector3 ChunkPositionFromVoxelCoordinates(World world, int x, int y, int z)
        {
            Vector3 pos = new Vector3
            {
                x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
                y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
                z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize
            };

            return pos;
        }
    }
}