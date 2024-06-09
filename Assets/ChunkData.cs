
using UnityEngine;

namespace DefaultNamespace
{
    public class ChunkData
    {
        public VoxelType[] voxels;
        public int chunkSize = 16;
        public int chunkHeight = 100;
        public World worldReference;
        public Vector3 worldPosition;

        public ChunkData( int chunkSize, int chunkHeight, World worldReference, Vector3 worldPosition)
        {
            this.voxels = new VoxelType[chunkSize * this.chunkHeight * chunkSize];
            this.chunkSize = chunkSize;
            this.chunkHeight = chunkHeight;
            this.worldReference = worldReference;
            this.worldPosition = worldPosition;
        }
    }
}