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
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentDestination = GetRandomPosition();
            print("currentDestination: " + currentDestination);
        }
    }

    private void FixedUpdate()
    {
        if (currentDestination == Vector3Int.zero) return;
        if (currentState != State.WANDERING) return;

        path = new List<Vector2Int>();
    }

    private Vector3Int GetRandomPosition()
    {
        // Currently selecting random position inside map,
        // maybe TODO: update the way this is selected to use the size of the village and travel inside the village
        Vector3Int raycastOrigin = new Vector3Int(
            Random.Range(-mapManager.MapSize.x / 2, mapManager.MapSize.x / 2),
            5,
            Random.Range(-mapManager.MapSize.y / 2, mapManager.MapSize.y / 2));
        Ray ray = new Ray(raycastOrigin, Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, lm))
        {
            throw new System.Exception("Raycast doesnt hit map");
        }
        
        return new Vector3Int(
            Mathf.RoundToInt(hit.point.x),
            Mathf.RoundToInt(hit.point.y),
            Mathf.RoundToInt(hit.point.z)
        );
    }
}