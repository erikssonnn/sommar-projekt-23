using System.Collections.Generic;
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

    private Vector3Int testedPosition;
    private ResourceManager resourceManager = null;

    public static BuildingManager Instance { get; set; }
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
        if (!buildingParent)
        {
            throw new System.Exception("BuildingParent was not set in BuildingManager");
        }

        resourceManager = ResourceManager.Instance;
        if (resourceManager == null)
        {
            throw new System.Exception("Cant find ResourceManager instance");
        }
        // TODO: Dirty fix for now!! Fix later??
        // Rotate -90 degrees on x axis to make the buildings face up
        previewBuildingRotation = Quaternion.Euler(-90, 0, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SelectBuilding(building);
        }
        
        if(!currentlySelectedBuilding) return;

        // Get a ray with mouse pointer position to world position
        Ray mouseToScreenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Raycast from mouse position to world position
        RaycastHit hit;
        if(!Physics.Raycast(mouseToScreenRay, out hit, Mathf.Infinity)) return;

        // Get the mouse position as an int
        Vector3Int mousePositionInt = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaceBuilding(mousePositionInt);
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectBuilding();
            Destroy(previewBuildingObject);
        }
        // Rotate the preview building
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            previewBuildingRotation *= Quaternion.Euler(0, 0, -90);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            previewBuildingRotation *= Quaternion.Euler(0, 0, -90);
        }
        
        PreviewBuilding(mousePositionInt);
    }

    /// <summary>
    /// Called from UI buttons to select a building type
    /// </summary>
    /// <param name="buildingType"> The type of building to be selected </param>
    public void SelectBuilding(Building buildingType)
    {
        // Throw error if no building type is selected
        if (!buildingType)
        {
            throw new System.Exception("BuildingType was set to null, please set a building type in UI button");
        }

        currentlySelectedBuilding = buildingType;
    }
    /// <summary>
    /// Called you want to deselect the currently selected building
    /// </summary>
    public void DeselectBuilding()
    {
        currentlySelectedBuilding = null;
    }

    private void PreviewBuilding(Vector3Int position)
    {
        if (!previewBuildingObject)
        {
            //Instanciate a new empty object with the currently selected building mesh but replace all the materials with preview material
            previewBuildingObject = Instantiate(currentlySelectedBuilding.obj, position, previewBuildingRotation);
            previewBuildingObject.transform.localScale *= 1.05f;
            previewBuildingObject.name = "Previewing " + currentlySelectedBuilding.name;
            Material[] mat = previewBuildingObject.GetComponent<MeshRenderer>().materials;
            // Loop through all the materials in the preview building and replace the materials with the preview material
            for (int i = 0; i < mat.Length; i++)
            {
                mat[i] = previewMaterial;
            }
            previewBuildingObject.GetComponent<MeshRenderer>().materials = mat;
        }
        else
        {
            if(testedPosition == position)
            {
                // Move the preview building to the mouse position
                previewBuildingObject.transform.position = position;
                previewBuildingObject.transform.rotation = previewBuildingRotation;
                if(CalculateIsOverlapping(previewBuildingObject, out _))
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

    // TODO: Convert overlapping check to job system
    private void PlaceBuilding(Vector3Int position)
    {
        // Instantiate a new building
        GameObject newBuilding = Instantiate(currentlySelectedBuilding.obj, position, previewBuildingRotation, buildingParent);
        newBuilding.name = currentlySelectedBuilding.name;

        if(CalculateIsOverlapping(newBuilding,out List<Vector2Int> positions))
        {
            // Destroy the new building
            Destroy(newBuilding);
            return;
        }
        // Add points to the map
        MapManager.Instance.OccupyPositions(positions);

        ResourceClass resourceCost = building.resourceRequirements;
        if (!resourceManager.HasEnoughResources(resourceCost)) return;
        resourceManager.ChangeResources(resourceCost);
        
        // FIXME: Spawn citizens
        // CitizenManager.Instance.SpawnCitizens(currentlySelectedBuilding.GetCitizensToSpawn(), position);

        // Destroy the preview building
        Destroy(previewBuildingObject);
    }

    private static bool CalculateIsOverlapping(GameObject newBuilding, out List<Vector2Int> buildingPositions)
    {
        // Get bounds of the building
        Bounds bounds = newBuilding.GetComponent<MeshRenderer>().bounds;

        Vector3Int transformPosition = new Vector3Int((int)newBuilding.transform.position.x, (int)newBuilding.transform.position.y, (int)newBuilding.transform.position.z);
        // Get all the positions that the building will occupy using half extends
        buildingPositions = new List<Vector2Int>();
        for (int x = (int)-bounds.extents.x; x < bounds.extents.x; x++)
        {
            for (int z = (int)-bounds.extents.z; z < bounds.extents.z; z++)
            {
                if(MapManager.Instance.IsObstructed(new Vector2Int(transformPosition.x + x, transformPosition.z + z)))
                {
                    // Later show a popup in corner of screen saying position is obstructed
                    Debug.LogWarning("Position is obstructed");
                    return true;
                }
                buildingPositions.Add(new Vector2Int(transformPosition.x + x, transformPosition.z + z));
            }
        }
        return false;
    }

    // On draw gizmos
    private void OnDrawGizmos()
    {
        if (!currentlySelectedBuilding) return;

        Ray mouseToScreenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.main.transform.position, mouseToScreenRay.direction * 100f);
    }
}
