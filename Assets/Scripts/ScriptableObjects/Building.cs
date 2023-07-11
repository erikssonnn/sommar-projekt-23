using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object for the building. Containing data like prefab object, resources needed, job type, dependancies in adjacent tiles
[CreateAssetMenu(fileName = "New Building", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : ScriptableObject
{
    [SerializeField] private string buildingName = null;
    [SerializeField] private GameObject buildingObject = null;
    // TODO: Add resources needed when resource manager is implemented
    // TODO: Add job type 
    // TODO: Add dependancies in adjacent tiles

    public string GetBuildingName()
    {
        return buildingName;
    }
    public GameObject GetObject()
    {
        return buildingObject;
    }
}
