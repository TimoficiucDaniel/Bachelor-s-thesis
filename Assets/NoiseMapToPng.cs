using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class NoiseMapToPng
    {
        public static void convertNoiseMapToPng(Texture2D texture, string fileName)
        {
            string filePath = Path.Combine(Application.dataPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Deleted existing file: {filePath}");
            }

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Saved PNG to: {filePath}");
        }
        
        public static float[,] loadNoiseMapFromPNG(string fileName)
        {
            string filePath = Path.Combine(Application.dataPath, fileName);

            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found: {filePath}");
                return null;
            }

            // Load the PNG file into a Texture2D
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            int width = texture.width;
            int height = texture.height;

            // Convert Texture2D to float[,] noise map
            float[,] noiseMap = new float[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = texture.GetPixel(x, y);
                    noiseMap[x, y] = 1-color.r; // Assuming grayscale, so r = g = b
                }
            }

            //noiseMap = ThermalErosion.normalizeHeightMap(width, height, noiseMap);
            return noiseMap;
        }
    }
}