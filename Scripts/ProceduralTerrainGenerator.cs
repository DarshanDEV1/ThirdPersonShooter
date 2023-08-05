using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    public int width = 256;       // Width of the terrain
    public int height = 256;      // Height of the terrain
    public float scale = 20.0f;   // Controls the level of detail
    public float amplitude = 10f; // Controls the height variation
    public int octaves = 5;       // Number of noise layers for more natural appearance
    public float lacunarity = 2f; // Frequency multiplier for each octave
    public float persistence = 0.5f; // Amplitude multiplier for each octave
    public Vector2 offset = new Vector2(0, 0); // Offset for noise sampling

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainMesh(terrain.terrainData);
    }

    TerrainData GenerateTerrainMesh(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, amplitude, height);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + offset.x;
                float yCoord = (float)y / height * scale + offset.y;

                float value = PerlinNoise(xCoord, yCoord);
                heights[x, y] = value;
            }
        }

        return heights;
    }

    float PerlinNoise(float x, float y)
    {
        float frequency = 1;
        float amplitude = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = x * frequency;
            float sampleY = y * frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return noiseHeight;
    }
}
