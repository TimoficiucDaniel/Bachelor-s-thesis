using DefaultNamespace;
using UnityEditor;
using UnityEngine;
[CustomEditor (typeof(World))]
public class WorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        World world = (World) target;
        if (GUILayout.Button("Generate"))
        {
            world.GenerateWorld();
            // VoxelDataManager.Load();
        }
    }
}