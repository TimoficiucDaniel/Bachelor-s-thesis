using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Voxel Data", menuName = "_Scripts/Voxel Data")]
    public class VoxelDataSO : ScriptableObject
    {
        public float textureSizeX, textureSizeY;
        public List<TextureData> textureDataList;
    }
    [Serializable]
    public class TextureData
    {
        public VoxelType voxelType;
        public Vector2 up, down, side;
        public bool isSolid = true;
        public bool generatesCollider = true;
    }
}