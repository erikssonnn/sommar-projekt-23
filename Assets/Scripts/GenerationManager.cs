using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GenerationManager : MonoBehaviour
{
    [SerializeField] private Transform mapParent = null;
    [SerializeField] private GameObject tile = null;
    [SerializeField] private Material[] terrainMaterials = null;
    
    private MapManager mapManager = null;
    private void Start()
    {
        NullChecker();
        GenerateGroundMesh();
        MergeMapMesh();
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

    private void MergeMapMesh()
    {
        List<MeshRenderer> allMeshes = new List<MeshRenderer>();
        if (mapParent.childCount > 0) {
            for (int i = mapParent.childCount - 1; i >= 0; i--) {
                allMeshes.Add(mapParent.GetChild(i).GetComponent<MeshRenderer>());
            }
        }

        Mesh finalMesh = new Mesh();
        CombineInstance[] combineInstance = new CombineInstance[allMeshes.Count];
        for (int i = 0; i < allMeshes.Count; i++) {
            combineInstance[i].mesh = allMeshes[i].GetComponent<MeshFilter>().sharedMesh;
            combineInstance[i].transform = allMeshes[i].transform.localToWorldMatrix;
        }

        finalMesh.CombineMeshes(combineInstance);
        GameObject newObject = new GameObject("mesh");
        MeshFilter filter = newObject.AddComponent<MeshFilter>();
        filter.sharedMesh = finalMesh;
        MeshRenderer renderer = newObject.AddComponent<MeshRenderer>();
        MeshCollider collider = newObject.AddComponent<MeshCollider>();

        for (int i = mapParent.childCount - 1; i >= 0; i--) {
            Transform child = mapParent.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        newObject.transform.SetParent(mapParent, false);
    }

    private void GenerateGroundMesh() {
        if (mapManager.Map.GetLength(0) == 0 || mapManager.Map.GetLength(1) == 0) return;
        int sizeX = mapManager.Map.GetLength(0);
        int sizeY = mapManager.Map.GetLength(1);
        
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                GameObject newTile = Instantiate(tile, mapParent, false);
                newTile.transform.position = new Vector3(x, 0, y);
                newTile.transform.eulerAngles = new Vector3(-90, 0, 0);

                if (y > 25)
                {
                    mapManager.Map[x, y].tileType = Tile.TileType.RIVER;
                }

                newTile.GetComponent<MeshRenderer>().material =
                    mapManager.Map[x, y].tileType == Tile.TileType.DEFAULT ? terrainMaterials[0] : terrainMaterials[1];
            }
        }
    }
    
    private void GenerateMapMesh()
    {
        if (mapManager.Map.GetLength(0) == 0 || mapManager.Map.GetLength(1) == 0) return;
        int sizeX = mapManager.Map.GetLength(0);
        int sizeY = mapManager.Map.GetLength(1);

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