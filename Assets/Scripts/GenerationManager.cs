using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Texture2D = UnityEngine.Texture2D;

[CustomEditor(typeof(GenerationManager))]
public class GenerationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GenerationManager generationManager = (GenerationManager)target;
        if (DrawDefaultInspector())
        {
            generationManager.GenerateMap();
        }

        if (GUILayout.Button("GENERATE"))
        {
            generationManager.GenerateMap();
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public enum RenderMode
{
    HEIGHTMAP,
    COLORMAP,
    MESH
};

public class MeshData
{
    public readonly Vector3[] vertices;
    private readonly int[] triangles;
    public readonly Vector2[] uvs;
    
    private int triangleIndex;
    
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        uvs = new Vector2[meshWidth * meshHeight];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}

public class GenerationManager : MonoBehaviour
{
    [Header("MAIN: ")]
    [SerializeField] private RenderMode renderMode = RenderMode.HEIGHTMAP;
    [SerializeField] private TerrainType[] terrainTypes = null;
    
    [Header("ASSIGNABLE: ")]
    [SerializeField] private Transform mapParent = null;

    [Header("NOISE: ")] 
    [SerializeField] private int seed = 0;
    [SerializeField] private bool randomSeed = false;
    [SerializeField] private float noiseScale = 1.0f;
    [Range(0, 5)][SerializeField] private int octaves = 0;
    [Range(0, 0.5f)][SerializeField] private float persistance = 0.0f;
    [Range(0, 25)][SerializeField] private float lacunarity = 0.0f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    private MapManager mapManager = null;
    private new Renderer renderer = null;
    private MeshRenderer meshRenderer = null;
    private MeshFilter meshFilter = null;

    private void NullChecker()
    {
        mapManager = GetComponent<MapManager>(); //DEBUG
        //mapManager = MapManager.Instance; //Cant get get instance in editor, only in play mode
        if (mapManager == null)
        {
            throw new System.Exception("Cant find mapManager instance!");
        }

        if (mapParent == null)
        {
            throw new System.Exception("mapParent object is null on " + name);
        }

        renderer = mapParent.GetComponent<Renderer>();
        meshRenderer = mapParent.GetComponent<MeshRenderer>();
        meshFilter = mapParent.GetComponent<MeshFilter>();
    }

    private float[,] GenerateNoise()
    {
        if (mapManager.MapSize.x == 0 || mapManager.MapSize.y == 0)
        {
            throw new System.Exception("MapSize is zero!");
        }

        int selectedSeed = randomSeed ? Random.Range(-10000, 10000) : seed;

        int sizeX = mapManager.MapSize.x;
        int sizeY = mapManager.MapSize.y;
        float maxNoiseHeight = float.MaxValue;
        float minNoiseHeight = float.MinValue;

        float[,] noise = new float[sizeX, sizeY];
        System.Random random = new System.Random(selectedSeed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = random.Next(-100000, 100000) + offset.x;
            float offsetY = random.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        noiseScale = Mathf.Clamp(noiseScale, 0.01f, 10000f);
        float halfWidth = sizeX / 2f;
        float halfHeight = sizeY / 2f;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noise[x, y] = noiseHeight;
            }
        }

        return noise;
    }

    public void GenerateMap()
    {
        // This was added here to have the terrain generator work in editor mode
        NullChecker();

        float[,] noiseMap = GenerateNoise();
        Color[] colorMap = new Color[mapManager.MapSize.x * mapManager.MapSize.y];
        
        for (int x = 0; x < mapManager.MapSize.x; x++)
        {
            for (int y = 0; y < mapManager.MapSize.y; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainTypes.Length; i++)
                {
                    if (!(currentHeight <= terrainTypes[i].height)) continue;
                    colorMap[y * mapManager.MapSize.x + x] = terrainTypes[i].color;
                    break;
                }
            }
        }

        switch (renderMode)
        {
            case RenderMode.HEIGHTMAP:
                RenderTexture(TextureFromHeightMap(noiseMap));
                break;
            case RenderMode.COLORMAP:
                RenderTexture(TextureFromColorMap(colorMap));
                break;
            case RenderMode.MESH:
                RenderMesh(GenerateMesh(noiseMap), TextureFromColorMap(colorMap));
                break;
            default:
                throw new System.Exception("Default switch case, this should not happen!");
        }
    }

    private Texture2D TextureFromColorMap(Color[] colorMap)
    {
        Texture2D texture2D = new Texture2D(mapManager.MapSize.x, mapManager.MapSize.y)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture2D.SetPixels(colorMap);
        texture2D.Apply();

        return texture2D;
    }
    
    private Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        Color[] color = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                color[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        
        return TextureFromColorMap(color);
    }

    private void RenderTexture(Texture2D texture2D)
    {
        renderer.sharedMaterial.mainTexture = texture2D;
        // renderer.transform.localScale = new Vector3(texture2D.width, 1, texture2D.height);
    }
    
    private void RenderMesh(MeshData meshData, Texture2D texture2D)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture2D;
    }

    private static MeshData GenerateMesh(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y], topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }
}