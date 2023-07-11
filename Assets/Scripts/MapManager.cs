using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3Int = UnityEngine.Vector3Int;

[System.Serializable]
public class Tile
{
    public enum TileType
    {
        DEFAULT,
        RIVER
    };

    public TileType tileType;

    public Tile(TileType tileType)
    {
        this.tileType = tileType;
    }
}

public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize = Vector2Int.zero;

    public Vector2Int MapSize
    {
        get => mapSize;
    }

    public static MapManager Instance { get; private set; }
    public Dictionary<Vector3Int, Tile> Map { get; private set; } = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsObstructed(Vector3Int position)
    {
        return Map.ContainsKey(position);
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
        foreach (Vector3Int t in positions.Where(t => !Map.ContainsKey(t) && !OutsideMapBounds(t)))
        {
            Map.Add(t, new Tile(Tile.TileType.DEFAULT));
        }
    }

    public void UnOccupyPositions(List<Vector3Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to remove zero new positions in " + name);
        }
        foreach (Vector3Int t in positions)
        {
            if (!Map.ContainsKey(t) || OutsideMapBounds(t))
            {
                throw new System.Exception("Tried to remove a non existing position in " + name);
            }
            Map.Remove(t);
        }
    }
}