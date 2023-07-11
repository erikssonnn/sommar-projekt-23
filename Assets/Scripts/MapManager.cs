using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3Int = UnityEngine.Vector3Int;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct ComputeOverlapJob : IJob
{
    [ReadOnly] public NativeArray<Vector3Int> alreadyOccupiedPositions;
    [ReadOnly] public Vector3Int testingPosition;
    [WriteOnly] public int nrOfOverlaps;

    public void Execute()
    { 
        // If the distance between the testing position and the occupied position is less than the radius, add it to the overlap positions
        if (alreadyOccupiedPositions.Contains(testingPosition))
        {
            //Debug.Log("Already occupied");
            nrOfOverlaps++;
            return; 
        }
    }
}

public class MapManager : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize = Vector2Int.zero;
    
    private static readonly List<Vector3Int> map = new List<Vector3Int>();
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

    private bool ComputeOverlap(Vector3Int[] positions)
    {
        float startTime = Time.realtimeSinceStartup;
        NativeArray<Vector3Int> occupiedPositions = new NativeArray<Vector3Int>(map.ToArray(), Allocator.TempJob);
        NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);
        int nrOfOverlaps = 0;
        for (int i = 0; i < positions.Length; i++)
        {
            jobHandles.Add(StartNewJob(occupiedPositions, positions[i], nrOfOverlaps));
        }
        JobHandle.CompleteAll(jobHandles);
        if(nrOfOverlaps > 0)
        {
            Debug.Log("Obstructed");
        }
        Debug.Log("ComputeOverlapJob took " + (Time.realtimeSinceStartup - startTime) * 1000 + "ms");
        jobHandles.Dispose();
        occupiedPositions.Dispose();
        return false;
    }

    private JobHandle StartNewJob(NativeArray<Vector3Int> occupiedPositions, Vector3Int positionToTest, int nrOfOverlaps)
    {
        ComputeOverlapJob job = new ComputeOverlapJob
        {
            alreadyOccupiedPositions = occupiedPositions,
            testingPosition = positionToTest,
            nrOfOverlaps = nrOfOverlaps
        };
        JobHandle jobHandle = job.Schedule();
        nrOfOverlaps = job.nrOfOverlaps;
        Debug.Log("Nr of overlaps: " + nrOfOverlaps);
        return jobHandle;
    }

    public bool IsObstructed(Vector3Int position)
    {
        return map.Contains(position);
    }
    public bool IsObstructed(Vector3Int[] positions)
    {
        return ComputeOverlap(positions);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3Int position in map)
        {
            Gizmos.DrawCube(position, Vector3.one);
        }
    }
}
