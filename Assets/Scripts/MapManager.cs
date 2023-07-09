using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3Int = UnityEngine.Vector3Int;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize = Vector2Int.zero;

    public static MapManager Instance { get; private set; }
    
    private readonly List<Vector3Int> map = new List<Vector3Int>();
    
    public Vector2Int MapSize
    {
        get => mapSize;
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

    public bool IsObstructed(Vector3Int position)
    {
        return map.Contains(position);
    }

    private bool OutsideMapBounds(Vector3Int position)
    {
        return position.x < -mapSize.x || position.x > mapSize.x ||
               position.z < -mapSize.y || position.z > mapSize.y;
    }

    public void AddPositions(List<Vector3Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to add zero new positions to the map");
        }
        foreach (Vector3Int t in positions.Where(t => !map.Contains(t) && !OutsideMapBounds(t)))
        {
            map.Add(t);
        }
    }

    public void RemovePositions(List<Vector3Int> positions)
    {
        if (!positions.Any())
        {
            throw new System.Exception("Tried to remove zero new positions in " + name);
        }
        foreach (Vector3Int t in positions)
        {
            if (!map.Contains(t) || OutsideMapBounds(t))
            {
                throw new System.Exception("Tried to remove a non existing position in " + name);
            }
            map.Remove(t);
        }
    }
}
