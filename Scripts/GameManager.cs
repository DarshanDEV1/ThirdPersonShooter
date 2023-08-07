using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int width = 256;       // Width of the terrain
    public int height = 256;      // Height of the terrain
    public float scale = 20.0f;   // Controls the level of detail
    public float amplitude = 10f; // Controls the height variation
    public int octaves = 5;       // Number of noise layers for more natural appearance
    public float lacunarity = 2f; // Frequency multiplier for each octave
    public float persistence = 0.5f; // Amplitude multiplier for each octave
    public Vector2 offset = new Vector2(0, 0); // Offset for noise sampling

    public TMP_Text _score_Text;
    public AudioClip[] _audioClips;
    public int _currentScore;
    private AudioSource _audioSource;

    [SerializeField] Terrain[] _terrains;
    [SerializeField] Transform[] _coinSpawnPosition;
    [SerializeField] GameObject _coinPrefab;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _audioClips[0];
        _audioSource.Play();
    }

    private void Start()
    {
        GenerateTerrain();
        SpawnCoinPositions();
    }

    private void SpawnCoinPositions()
    {
        for (int i = 0; i < _coinSpawnPosition.Length; i++)
        {
            int x = Random.Range(3, 12);
            for (int j = 0; j < x; j++)
            {
                Vector3 newPosition = new Vector3(_coinSpawnPosition[i].position.x + Random.Range(-50, 50),
                                                  5f,
                                                  _coinSpawnPosition[i].position.z + Random.Range(-50, 50));
                Instantiate(_coinPrefab, newPosition, Quaternion.identity);
            }
        }
    }

    #region TERRAIN_SECTION
    void GenerateTerrain()
    {
        foreach (Terrain terrain in _terrains)
        {
            terrain.terrainData = GenerateTerrainMesh(terrain.terrainData);
        }
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
    #endregion
}