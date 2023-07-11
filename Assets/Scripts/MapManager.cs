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
    private readonly Dictionary<Vector2Int, Tile> map = new Dictionary<Vector2Int, Tile>();

    private void OnDrawGizmos()
    {
        if (map == null || map.Count == 0) return;
        foreach (Vector2Int position in map.Select(tile => tile.Key))
        {
            Gizmos.color = Color.red;
            // TODO: find out why this is offset by (1, 1)
            Gizmos.DrawCube(new Vector3(position.x + 1, 0, position.y + 1), Vector3.one);
        }
    }

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

    public bool IsObstructed(Vector2Int position)
    {
        return map.ContainsKey(position);
    }

    private bool OutsideMapBounds(Vector2Int position)
    {
        return position.x < -mapSize.x || position.x > mapSize.x ||
               position.y < -mapSize.y || position.y > mapSize.y;
    }

    public void OccupyPositions(List<Vector2Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to add zero new positions to the map");
        }
        foreach (Vector2Int t in positions.Where(t => !map.ContainsKey(t) && !OutsideMapBounds(t)))
        {
            map.Add(t, new Tile(Tile.TileType.DEFAULT));
        }
    }

    public void UnOccupyPositions(List<Vector2Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to remove zero new positions in " + name);
        }
        foreach (Vector2Int t in positions)
        {
            if (!map.ContainsKey(t) || OutsideMapBounds(t))
            {
                throw new System.Exception("Tried to remove a non existing position in " + name);
            }
            map.Remove(t);
        }
    }
}