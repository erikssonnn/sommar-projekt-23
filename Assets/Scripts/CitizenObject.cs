using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using Vector3Int = UnityEngine.Vector3Int;

public enum State
{
    IDLE,
    WANDERING
}

public class PriorityQueue<T> where T : System.IComparable<T>
{
    private readonly List<T> elements = new List<T>();
    public int Count => elements.Count;

    public void Enqueue(T item)
    {
        elements.Add(item);
        elements.Sort();
    }

    public T Dequeue()
    {
        if (elements.Count == 0)
        {
            Debug.LogWarning("PriorityQueue is empty.");
            return default;
        }

        T item = elements[0];
        elements.RemoveAt(0);
        return item;
    }

    public bool Contains(T item)
    {
        return elements.Contains(item);
    }

    public void Clear()
    {
        elements.Clear();
    }
}

public class CitizenObject : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 0.0f;
    [SerializeField] private State currentState = State.IDLE;
    [SerializeField] private LayerMask lm = 0;

    private Vector3Int currentDestination = Vector3Int.zero;
    private MapManager mapManager = null;
    private List<Vector2Int> path = new List<Vector2Int>();

    private void Start()
    {
        NullChecker();
    }

    private void OnDrawGizmos()
    {
        // TODO: remove, debug
        if (currentDestination == Vector3Int.zero) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(currentDestination.x, 5, currentDestination.y), Vector3.one);

        if (path.Count == 0) return;
        foreach (Vector2Int point in path)
        {
            Gizmos.DrawCube(new Vector3(point.x, 5, point.y), Vector3.one);
        }
    }

    private void NullChecker()
    {
        mapManager = MapManager.Instance;
        if (mapManager == null)
        {
            throw new System.Exception("Cant find MapManager Instance");
        }
    }

    private void Update()
    {
        // TODO: remove, debug
        if (!Input.GetKeyDown(KeyCode.P)) return;
        currentDestination = GetRandomPosition();
        print("currentDestination: " + currentDestination);
    }

    private void FixedUpdate()
    {
        if (currentDestination == Vector3Int.zero) return;
        if (currentState != State.WANDERING) return;

        path = FindPath(transform.position, currentDestination);

        if (path.Count <= 0) return;
        StartCoroutine(FollowPath(path));
    }

    private List<Vector2Int> FindPath(Vector3 startPos, Vector3Int targetPos)
    {
        Vector2Int start = new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.z));
        Vector2Int target = new Vector2Int(targetPos.x, targetPos.z);

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<PathNode> openSet = new PriorityQueue<PathNode>();
        openSet.Enqueue(new PathNode(start, 0, CalculateHeuristic(start, target), null));

        while (openSet.Count > 0)
        {
            PathNode current = openSet.Dequeue();

            if (current.position == target)
            {
                return ReconstructPath(current);
            }

            closedSet.Add(current.position);

            foreach (PathNode neighborNode in from neighbor 
                         in GetNeighbors(current.position) 
                     where !closedSet.Contains(neighbor) 
                     let gScore = current.gScore + 1 
                     let hScore = CalculateHeuristic(neighbor, target) 
                     let fScore = gScore + hScore 
                     let neighborNode = new PathNode(neighbor, gScore, hScore, current) 
                     where !openSet.Contains(neighborNode) || fScore < neighborNode.fScore 
                     select neighborNode)
            {
                openSet.Enqueue(neighborNode);
            }
        }

        return new List<Vector2Int>();
    }

    private static int CalculateHeuristic(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1),
            new Vector2Int(position.x + 1, position.y + 1),
            new Vector2Int(position.x + 1, position.y - 1),
            new Vector2Int(position.x - 1, position.y + 1),
            new Vector2Int(position.x - 1, position.y - 1)
        };

        neighbors.RemoveAll(n => !mapManager.IsObstructed(n));
        return neighbors;
    }

    private static List<Vector2Int> ReconstructPath(PathNode node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode current = node;

        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private System.Collections.IEnumerator FollowPath(IReadOnlyList<Vector2Int> path)
    {
        for (int i = 1; i < path.Count; i++)
        {
            Vector3Int nextPosition = new Vector3Int(path[i].x, 0, path[i].y);
            while (Vector3.Distance(transform.position, nextPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    nextPosition,
                    Time.deltaTime * movementSpeed);
                yield return null;
            }
        }
    }

    private class PathNode : System.IComparable<PathNode>
    {
        public Vector2Int position;
        public int gScore;
        public int hScore;
        public int fScore => gScore + hScore;
        public PathNode parent;

        public PathNode(Vector2Int position, int gScore, int hScore, PathNode parent)
        {
            this.position = position;
            this.gScore = gScore;
            this.hScore = hScore;
            this.parent = parent;
        }

        public int CompareTo(PathNode other)
        {
            return fScore.CompareTo(other.fScore);
        }
    }

    private Vector3Int GetRandomPosition()
    {
        // Currently selecting random position inside map,
        // maybe TODO: update the way this is selected to use the size of the village and travel inside the village
        // Vector3Int raycastOrigin = new Vector3Int(
        //     Random.Range(-mapManager.MapSize.x / 2, mapManager.MapSize.x / 2),
        //     50,
        //     Random.Range(-mapManager.MapSize.y / 2, mapManager.MapSize.y / 2));
        // Ray ray = new Ray(raycastOrigin, Vector3.down);
        // Debug.DrawRay(raycastOrigin, Vector3.down, Color.magenta, Mathf.Infinity);
        // if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, lm))
        // {
        //     throw new System.Exception("Raycast doesnt hit map");
        // }
        //
        // return new Vector3Int(
        //     Mathf.RoundToInt(hit.point.x),
        //     Mathf.RoundToInt(hit.point.y),
        //     Mathf.RoundToInt(hit.point.z)
        // );

        return new Vector3Int(Random.Range(0, 25), 5, Random.Range(0, 25));
    }
}