using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Vector3Int = UnityEngine.Vector3Int;

[System.Serializable]
public class Tile
{
    public enum TileType
    {
        DEFAULT,
        RIVER
    };

    public bool occupied;
    public Vector3Int position;
    public TileType tileType;

    public Tile(bool occupied, Vector3Int position, TileType tileType)
    {
        this.occupied = occupied;
        this.position = position;
        this.tileType = tileType;
    }
}

public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize = Vector2Int.zero;

    public static MapManager Instance { get; private set; }
    public Tile[,] Map { get; private set; } = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateMap();
    }

    private void CreateMap()
    {
        Map = new Tile[mapSize.x, mapSize.y];

        for (int x = 0; x < Map.GetLength(0); x++)
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                Map[x, y] = new Tile(false, new Vector3Int(x, 0, y), Tile.TileType.DEFAULT);
            }
        }
    }

    public bool IsObstructed(Vector3Int position)
    {
        for (int x = 0; x < Map.GetLength(0); x++)
        {
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                if (Map[x, y].position == position)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool OutsideMapBounds(Vector3Int position)
    {
        return position.x < -mapSize.x || position.x > mapSize.x ||
               position.z < -mapSize.y || position.z > mapSize.y;
    }

    public void OccupyPositions(List<Vector3Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to add zero new positions to the map");
        }

        foreach (Vector3Int pos in positions)
        {
            bool foundPosition = false;
            
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    if (Map[x, y].position != pos) continue;
                    foundPosition = true;
                    Map[x, y].occupied = true;
                }
            }

            if (!foundPosition)
            {
                throw new System.Exception("Tried to occupy a position that is not present in the map grid");
            }
        }
    }

    public void UnOccupyPositions(List<Vector3Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to remove zero new positions in " + name);
        }

        foreach (Vector3Int pos in positions)
        {
            bool foundPosition = false;
            
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    if (Map[x, y].position != pos) continue;
                    foundPosition = true;
                    Map[x, y].occupied = false;
                }
            }

            if (!foundPosition)
            {
                throw new System.Exception("Tried to unoccupy a position that is not present in the map grid");
            }
        }
    }
}