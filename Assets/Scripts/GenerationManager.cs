using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GenerationManager : MonoBehaviour
{
    [SerializeField] private Transform mapParent = null;
    [SerializeField] private Material[] terrainMaterials = null;
    
    private MapManager mapManager = null;
    private void Start()
    {
        NullChecker();
        GenerateMapMesh();
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
        if (terrainMaterials.Length == 0)
        {
            throw new System.Exception("terrainMaterials array is empty on " + name);
        }
    }

    private void GenerateMapMesh()
    {
        if (mapManager.MapSize.x == 0 || mapManager.MapSize.y == 0) return;
        int sizeX = mapManager.MapSize.x;
        int sizeY = mapManager.MapSize.y;

        GameObject meshObject = new GameObject("terrainMesh");
        meshObject.transform.eulerAngles = new Vector3(180f, 0, 0);
        meshObject.transform.SetParent(mapParent, false);
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterials[0];
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int vertexIndex = 0;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3[] currentFace = {
                    new Vector3(0 + x, 0, 0 + y),
                    new Vector3(1 + x, 0, 0 + y),
                    new Vector3(1 + x, 0, 1 + y),
                    new Vector3(0 + x, 0, 1 + y)
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