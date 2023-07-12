using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2Int = UnityEngine.Vector2Int;

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
    
    private static readonly List<Vector2Int> map = new List<Vector2Int>();
    public static MapManager Instance { get; set; }
    
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
        return Map.ContainsKey(position);
    }

    private bool OutsideMapBounds(Vector2Int position)
    {
        return position.x < -mapSize.x || position.x > mapSize.x ||
               position.y < -mapSize.y || position.y > mapSize.y;
    }

    public void AddPositions(List<Vector2Int> positions)
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

    public void RemovePositions(List<Vector2Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to remove zero new positions in " + name);
        }
        foreach (Vector2Int t in positions)
        {
            if (!Map.ContainsKey(t) || OutsideMapBounds(t))
            {
                throw new System.Exception("Tried to remove a non existing position in " + name);
            }
            Map.Remove(t);
        }
    }
}