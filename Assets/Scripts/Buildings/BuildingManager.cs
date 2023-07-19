using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private Material previewMaterial = null;
    [SerializeField] private Transform buildingParent = null;
    [SerializeField] private Color previewColor = Color.white;
    [SerializeField] private Color obstructedColor = Color.red;
    [SerializeField] private Building building = null;

    private Building currentlySelectedBuilding = null;
    private GameObject previewBuildingObject = null;
    private Quaternion previewBuildingRotation = Quaternion.identity;

    private Vector3Int testedPosition = Vector3Int.zero;
    private ResourceManager resourceManager = null;
    private new Camera camera = null;

    public static BuildingManager Instance { get; private set; }

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

    private void Start()
    {
        camera = Camera.main;
        if (camera == null)
        {
            throw new System.Exception("Cant find main camera");
        }

        if (!buildingParent)
        {
            throw new System.Exception("BuildingParent was not set in BuildingManager");
        }

        resourceManager = ResourceManager.Instance;
        if (resourceManager == null)
        {
            throw new System.Exception("Cant find ResourceManager instance");
        }

        previewBuildingRotation = Quaternion.Euler(-90, 0, 0);
    }

    private void Update()
    {
        if (!currentlySelectedBuilding) return;

        Ray mouseToScreenRay = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(mouseToScreenRay, out RaycastHit hit, Mathf.Infinity)) return;

        Vector3Int mousePositionInt = new Vector3Int(
            Mathf.RoundToInt(hit.point.x),
            Mathf.RoundToInt(hit.point.y),
            Mathf.RoundToInt(hit.point.z));

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaceBuilding(mousePositionInt);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CancelBuilding();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            previewBuildingRotation *= Quaternion.Euler(0, 0, -90);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            previewBuildingRotation *= Quaternion.Euler(0, 0, -90);
        }

        PreviewBuilding(mousePositionInt);
    }

    public void SelectBuilding(Building buildingType)
    {
        if (!buildingType)
        {
            throw new System.Exception("BuildingType was set to null, please set a building type in UI button");
        }

        ResourceClass resourceCost = building.resourceRequirements;
        if (!resourceManager.HasEnoughResources(resourceCost)) return;
        currentlySelectedBuilding = buildingType;
    }

    private void CancelBuilding()
    {
        currentlySelectedBuilding = null;
        Destroy(previewBuildingObject);
    }

    private void PreviewBuilding(Vector3Int position)
    {
        if (!previewBuildingObject)
        {
            previewBuildingObject = Instantiate(currentlySelectedBuilding.obj, position, previewBuildingRotation);
            previewBuildingObject.transform.localScale *= 1.05f;
            previewBuildingObject.name = "Previewing " + currentlySelectedBuilding.name;

            Material[] meshMaterials = previewBuildingObject.GetComponent<MeshRenderer>().materials;
            meshMaterials = meshMaterials.Select(mat => previewMaterial).ToArray();
            previewBuildingObject.GetComponent<MeshRenderer>().materials = meshMaterials;
        }
        else
        {
            if (testedPosition == position)
            {
                previewBuildingObject.transform.position = position;
                previewBuildingObject.transform.rotation = previewBuildingRotation;
                if (CalculateIsOverlapping(previewBuildingObject, out _))
                {
                    foreach (Material t in previewBuildingObject.GetComponent<MeshRenderer>().materials)
                    {
                        t.color = obstructedColor;
                    }
                }
                else
                {
                    foreach (Material t in previewBuildingObject.GetComponent<MeshRenderer>().materials)
                    {
                        t.color = previewColor;
                    }
                }
            }

            testedPosition = position;
        }
    }

    private void PlaceBuilding(Vector3Int position)
    {
        ResourceClass resourceCost = building.resourceRequirements;
        if (!resourceManager.HasEnoughResources(resourceCost)) return;
        if (CalculateIsOverlapping(previewBuildingObject, out List<Vector2Int> positions)) return;

        GameObject newBuilding =
            Instantiate(currentlySelectedBuilding.obj, position, previewBuildingRotation, buildingParent);
        newBuilding.name = currentlySelectedBuilding.name;

        MapManager.Instance.OccupyPositions(positions);
        resourceManager.ChangeResources(resourceCost);

        // TODO: Spawn citizens

        if (!resourceManager.HasEnoughResources(resourceCost))
        {
            CancelBuilding();
        }
        Destroy(previewBuildingObject);
    }

    private static bool CalculateIsOverlapping(GameObject newBuilding, out List<Vector2Int> buildingPositions)
    {
        Bounds bounds = newBuilding.GetComponent<MeshRenderer>().bounds;
        Transform t = newBuilding.transform;

        Vector3Int transformPosition = new Vector3Int(Mathf.RoundToInt(t.position.x),
            Mathf.RoundToInt(t.position.y), Mathf.RoundToInt(t.position.z));

        buildingPositions = new List<Vector2Int>();
        for (int x = (int)-bounds.extents.x; x < bounds.extents.x; x++)
        {
            for (int z = (int)-bounds.extents.z; z < bounds.extents.z; z++)
            {
                if (MapManager.Instance.IsObstructed(new Vector2Int(transformPosition.x + x, transformPosition.z + z)))
                {
                    Debug.LogWarning("Position is obstructed");
                    return true;
                }

                buildingPositions.Add(new Vector2Int(transformPosition.x + x, transformPosition.z + z));
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!currentlySelectedBuilding) return;

        Ray mouseToScreenRay = camera.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(camera.transform.position, mouseToScreenRay.direction * 100f);
    }
}