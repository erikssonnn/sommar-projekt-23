using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
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
    private Building CurrentlySelectedBuilding = null;

    /// <summary>
    /// Called from UI buttons to select a building type
    /// </summary>
    /// <param name="BuildingType"> The type of building to be selected </param>
    public void SelectedBuilding(Building BuildingType)
    {
        Debug.Log("Selected Building: " + BuildingType.GetBuildingName());
        CurrentlySelectedBuilding = BuildingType;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!CurrentlySelectedBuilding) return;

    }

    private void PreviewBuilding()
    {
        
    }

}
