using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private Material PreviewMaterial = null;

    [SerializeField] private Transform BuildingParent = null;
    private Building CurrentlySelectedBuilding = null;
    private GameObject PreviewBuildingObject = null;
    private Quaternion PreviewBuildingRotation = Quaternion.identity;


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
        if (!BuildingParent)
        {
            throw new System.Exception("BuildingParent was not set in BuildingManager");
        }
        // Rotate -90 degrees on x axis to make the buildings face up
        PreviewBuildingRotation = Quaternion.Euler(-90, 0, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!CurrentlySelectedBuilding) return;

        // Get a ray with mouse pointer position to world position
        Ray MouseToScreenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Raycast from mouse position to world position
        RaycastHit Hit;
        bool DidHit = Physics.Raycast(MouseToScreenRay, out Hit, Mathf.Infinity);
        if(!DidHit) return;

        // Get the mouse position as an int
        Vector3Int MousePositionInt = new Vector3Int((int)Hit.point.x, (int)Hit.point.y, (int)Hit.point.z);

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaceBuilding(MousePositionInt);
        }
        // Rotate the preview building
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            PreviewBuildingRotation *= Quaternion.Euler(0, 0, -90);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            PreviewBuildingRotation *= Quaternion.Euler(0, 0, -90);
        }
        
        PreviewBuilding(MousePositionInt);
    }

    /// <summary>
    /// Called from UI buttons to select a building type
    /// </summary>
    /// <param name="BuildingType"> The type of building to be selected </param>
    public void SelectedBuilding(Building BuildingType)
    {
        // Throw error if no building type is selected
        if (!BuildingType)
        {
            throw new System.Exception("BuildingType was set to null, please set a building type in UI button");
        }

        CurrentlySelectedBuilding = BuildingType;
    }

    private void PreviewBuilding(Vector3Int Position)
    {
        if (!PreviewBuildingObject)
        {
            //Instanciate a new empty object with the currently selected building mesh but replace all the materials with preview material
            PreviewBuildingObject = Instantiate(CurrentlySelectedBuilding.GetObject(), Position, PreviewBuildingRotation);
            PreviewBuildingObject.name = "Previewing " + CurrentlySelectedBuilding.GetBuildingName();
            Material[] mat = PreviewBuildingObject.GetComponent<MeshRenderer>().materials;
            // Loop through all the materials in the preview building and replace the materials with the preview material
            for (int i = 0; i < mat.Length; i++)
            {
                mat[i] = PreviewMaterial;
            }
            PreviewBuildingObject.GetComponent<MeshRenderer>().materials = mat;
        }
        else
        {
            // Move the preview building to the mouse position
            PreviewBuildingObject.transform.position = Position;
            PreviewBuildingObject.transform.rotation = PreviewBuildingRotation;
        }
    }

    private void PlaceBuilding(Vector3Int Position)
    {
        // Check if the position is valid

        // Check if resources are available

        // Instanciate a new building
        GameObject NewBuilding = Instantiate(CurrentlySelectedBuilding.GetObject(), Position, PreviewBuildingRotation, BuildingParent);
        NewBuilding.name = CurrentlySelectedBuilding.GetBuildingName();
        // Destroy the preview building
        Destroy(PreviewBuildingObject);
    }

    // On draw gizmos
    private void OnDrawGizmos()
    {
        if (!CurrentlySelectedBuilding) return;

        Ray MouseToScreenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.main.transform.position, MouseToScreenRay.direction * 100f);
    }
}
