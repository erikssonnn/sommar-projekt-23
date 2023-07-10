using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GenerationManager : MonoBehaviour
{
    [SerializeField] private Transform mapParent = null;
    [SerializeField] private float noiseScale = 1.0f;
    
    private MapManager mapManager = null;
    private new Renderer renderer;
    
    private void Start()
    {
        NullChecker();
        GenerateMap();
    }

    private void NullChecker()
    {
        
        mapManager = MapManager.Instance;
        if (mapManager == null)
        {
            throw new System.Exception("Cant find mapManager instance!");
        }
        if (mapParent == null)
        {
            throw new System.Exception("mapParent object is null on " + name);
        }
        renderer = mapParent.GetComponent<Renderer>();
    }

    private float[,] GenerateNoise(float scale)
    {
        if (mapManager.MapSize.x == 0 || mapManager.MapSize.y == 0)
        {
            throw new System.Exception("MapSize is zero!");
        }
        
        int sizeX = mapManager.MapSize.x;
        int sizeY = mapManager.MapSize.y;
        float[,] noise = new float[sizeX, sizeY];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noise[x, y] = perlinValue;
            }
        }
        
        return noise;
    }

    private void GenerateMap()
    {
        float[,] noise = GenerateNoise(noiseScale);
        RenderNoise(noise);
    }

    private void RenderNoise(float[,] noise)
    {
        int width = noise.GetLength(0);
        int height = noise.GetLength(1);

        Texture2D texture2D = new Texture2D(width, height);
        Color[] color = new Color[width * height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                color[y * width + x] = Color.Lerp(Color.black, Color.white, noise[x, y]);
            }
        }
        
        texture2D.SetPixels(color);
        texture2D.Apply();
        renderer.sharedMaterial.mainTexture = texture2D;
        renderer.transform.localScale = new Vector3(width, 1, height);
    }
    
    private void GenerateFlatMesh()
    {
        if (mapManager.MapSize.x == 0 ||mapManager.MapSize.y == 0) return;
        int sizeX = mapManager.MapSize.x;
        int sizeY = mapManager.MapSize.y;

        GameObject meshObject = new GameObject("terrainMesh");
        meshObject.transform.eulerAngles = new Vector3(180f, 0, 0);
        meshObject.transform.SetParent(mapParent, false);
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int vertexIndex = 0;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3[] currentFace = {
                    new Vector3(x, 0, y),
                    new Vector3(1 + x, 0, y),
                    new Vector3(1 + x, 0, 1 + y),
                    new Vector3(x, 0, 1 + y)
                };
                
                vertices.Add(currentFace[0]);
                vertices.Add(currentFace[1]);
                vertices.Add(currentFace[2]);
                vertices.Add(currentFace[3]);
                
                triangles.Add(0 + vertexIndex);
                triangles.Add(1 + vertexIndex);
                triangles.Add(2 + vertexIndex);
                triangles.Add(0 + vertexIndex);
                triangles.Add(2 + vertexIndex);
                triangles.Add(3 + vertexIndex);

                vertexIndex += 4;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
        print("Generated terrain mesh with " + vertices.Count + " vertices!");
    }
}