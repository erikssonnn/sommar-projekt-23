using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object for the building. Containing data like mesh, job type, dependancies in adjacent tiles
[CreateAssetMenu(fileName = "New Building", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : ScriptableObject
{
    // The name of the building
    [SerializeField] private string BuildingName = null;
    [SerializeField] private Mesh BuildingMesh = null;
    // For when we add job types
    // [SerializeField] private Jobtype = null;

    public string GetBuildingName()
    {
        return BuildingName;
    }
}
