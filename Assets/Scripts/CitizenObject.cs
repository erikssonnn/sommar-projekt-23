using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using Vector3Int = UnityEngine.Vector3Int;

public enum State { IDLE, WANDERING }

public class CitizenObject : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 0.0f;
    [SerializeField] private State currentState = State.IDLE;
    [SerializeField] private LayerMask lm = 0;
    
    private Vector3Int currentDestination = Vector3Int.zero;
    private MapManager mapManager = null;
    private Vector3Int debugPos = Vector3Int.zero;

    private void Start()
    {
        NullChecker();
        currentDestination = GetRandomPosition();
    }

    private void NullChecker()
    {
        mapManager = MapManager.Instance;
        if (mapManager == null)
        {
            throw new System.Exception("Cant find MapManager Instance");
        }
    }

    // private void OnDrawGizmos()
    // {
    //     if (debugPos == Vector3Int.zero)
    //     {
    //         MapManager m = FindObjectOfType<MapManager>();
    //         debugPos = new Vector3Int(
    //             Random.Range(-m.MapSize.x / 2, m.MapSize.x / 2),
    //             50,
    //             Random.Range(-m.MapSize.y / 2, m.MapSize.y / 2));
    //     }
    //
    //     Gizmos.color = Color.red;
    //     print(debugPos);
    //     Gizmos.DrawRay(debugPos, Vector3.down * 100f);
    // }

    private void FixedUpdate()
    {
        if (currentDestination == Vector3Int.zero) return;
        if (currentState != State.WANDERING) return;

        Vector3 pos = transform.position;
        
        Ray ray = new Ray(transform.position, Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            throw new System.Exception("Raycast doesnt hit map");
        }

        pos += transform.forward * movementSpeed * Time.fixedDeltaTime;
        pos.y = hit.point.y + 0.5f;
        transform.position = pos;
        
        float dist = Vector3.Distance(transform.position, currentDestination);
        if (!(dist < 0.1f)) return;
        currentDestination = Vector3Int.zero;
        currentDestination = GetRandomPosition();
    }

    private Vector3Int GetRandomPosition()
    {
        // Currently selecting random position inside map,
        // maybe TODO: update the way this is selected to use the size of the village and travel inside the village
        Vector3Int raycastOrigin = new Vector3Int(
            Random.Range(-mapManager.MapSize.x / 2, mapManager.MapSize.x / 2),
            50,
            Random.Range(-mapManager.MapSize.y / 2, mapManager.MapSize.y / 2));
        Ray ray = new Ray(raycastOrigin, Vector3.down);
        Debug.DrawRay(raycastOrigin, Vector3.down, Color.magenta, Mathf.Infinity);
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