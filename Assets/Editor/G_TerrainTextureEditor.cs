using UnityEngine;
using UnityEditor;
using System.Linq;

public class G_TerrainTextureEditor
{
    [MenuItem("Terrain/Terrain Texture Editor")]
    public static void ApplyTexture()
    {
        Undo.RegisterCompleteObjectUndo(Terrain.activeTerrain.terrainData, "Terrain Texture Editor");
        int steepLayerIndex = 1; // Layer to apply on steep areas
        int lowHeightLayerIndex = 2; // Layer to apply on low altitude areas
        int highHeightLayerIndex = 0; // Layer to apply on high altitude areas
        float minStepAngle = 30f; // Angle above which the terrain is considered steep
        float maxStepAngle = 50f; // Angle above which the terrain is considered maximum steep
        float minHeight = 3f; // Altitude below which the terrain is considered low
        float maxHeight = 7f; // Altitude above which the terrain is considered high

        TerrainData terrainData = Terrain.activeTerrain.terrainData; // Get TerrainData
        int width = terrainData.alphamapWidth; // Get alphamap width
        int height = terrainData.alphamapHeight; // Get alphamap height

        // Get alphamap
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, width, height);

        // Traverse all pixels of the alphamap
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Get the steepness of the terrain
                float steepness = terrainData.GetSteepness(y / (float)height, x / (float)width);
                // Get the altitude
                float altitude = terrainData.GetInterpolatedHeight(y / (float)height, x / (float)width);

                float[] alphas = Enumerable.Repeat(0f, terrainData.alphamapLayers).ToArray();

                float stepAlpha = Mathf.Clamp01((steepness - minStepAngle) / (maxStepAngle - minStepAngle));
                float normalAlpha = Mathf.Clamp01((altitude - minHeight) / (maxHeight - minHeight));
                float lowHeightAlpha = 1 - normalAlpha;

                alphas[0] = normalAlpha * (1-stepAlpha);
                alphas[1] = stepAlpha;
                alphas[2] = lowHeightAlpha * (1 - stepAlpha);

                // Traverse all layers
                for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                {
                    alphaMaps[x, y, layer] = alphas[layer];
                }
            }
        }

        // Update the alphamap
        terrainData.SetAlphamaps(0, 0, alphaMaps);
    }

    [MenuItem("Terrain/Terrain Grass Editor")]
    public static void EditGrass()
    {
        Undo.RegisterCompleteObjectUndo(Terrain.activeTerrain.terrainData, "Terrain Grass Editor");

        int growLayer = 0;

        TerrainData terrainData = Terrain.activeTerrain.terrainData; // Get TerrainData

        // Loop through each point on the terrain
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Get the current alpha map at this point
                float[,,] alphamaps = terrainData.GetAlphamaps(x, y, 1, 1);

                // Check if the strength of layer 1 is the highest
                if (alphamaps[0, 0, growLayer] == alphamaps.Cast<float>().Max())
                {
                    // Set the detail at this point
                    int[,] newDetail = new int[,] { { 15 } };
                    terrainData.SetDetailLayer(x, y, 0, newDetail);
                    Debug.Log(x + "," + y);
                }
            }
        }
}
}
