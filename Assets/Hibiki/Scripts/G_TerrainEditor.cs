using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

public class G_TerrainEditor : MonoBehaviour
{
    
}

#if UNITY_EDITOR

[CustomEditor(typeof(G_TerrainEditor))]
public class G_TerrainEditorEditor : Editor
{
    private int steepLayerIndex = 1; // Layer to apply on steep areas
    private int lowHeightLayerIndex = 2; // Layer to apply on low altitude areas
    private int highHeightLayerIndex = 0; // Layer to apply on high altitude areas
    private float minStepAngle = 30f; // Angle above which the terrain is considered steep
    private float maxStepAngle = 50f; // Angle above which the terrain is considered maximum steep
    private float minHeight = 3f; // Altitude below which the terrain is considered low
    private float maxHeight = 7f; // Altitude above which the terrain is considered high

    private G_TerrainEditor terrainEditor;
    private Terrain terrain;
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Textureを地形に合わせる"))
        {
            SetTexture();
        }
        if (GUILayout.Button("草を配置"))
        {
            SetGrass();
        }
    }

    private G_TerrainEditor GetTarget()
    {
        if (!terrainEditor)
        {
            terrainEditor = target as G_TerrainEditor;
        }
        return terrainEditor;
    }
    private Terrain GetTerrain()
    {
        if (!terrain)
        {
            terrain = GetTarget().GetComponent<Terrain>();
        }
        return terrain;
    }

    public void SetGrass()
    {
        int detailResolution = 1024;
        int detailResolutionPatch = 16;
        GetTerrain().terrainData.SetDetailResolution(detailResolution, detailResolutionPatch);

        int[,] map = terrain.terrainData.GetDetailLayer(0, 0, detailResolution, detailResolution, 0);

        TerrainData terrainData = GetTerrain().terrainData;
        int width = terrainData.alphamapWidth; // Get alphamap width
        int height = terrainData.alphamapHeight; // Get alphamap height

        // Get alphamap
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, width, height);

        float mapY = height / (float)detailResolution;
        float mapX = width / (float)detailResolution;
        for (int y = 0; y < detailResolution; y++)
        {
            int pointY = Mathf.FloorToInt(mapY * y);
            for (int x = 0; x < detailResolution; x++)
            {
                int pointX = Mathf.FloorToInt(mapX * x);
                map[x, y] = Mathf.RoundToInt(250f * alphaMaps[pointX, pointY, 0]);
            }
        }
        GetTerrain().terrainData.SetDetailLayer(0, 0, 0, map);
        EditorUtility.SetDirty(GetTarget());
    }

    public void SetTexture()
    {
        Undo.RegisterCompleteObjectUndo(Terrain.activeTerrain.terrainData, "Terrain Texture Editor");
        

        TerrainData terrainData = GetTerrain().terrainData;
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

                alphas[0] = normalAlpha * (1 - stepAlpha);
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
        EditorUtility.SetDirty(GetTarget());
    }
}


#endif