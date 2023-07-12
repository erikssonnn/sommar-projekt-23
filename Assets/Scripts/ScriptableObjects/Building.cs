using UnityEngine;

[System.Serializable]
public struct ResourceRequirement
{
    // TODO: Switch from string to enum when resource manager is implemented
    public string resourceName;
    public int amount;
}

//Scriptable object for the building. Containing data like prefab object, resources needed, job type, dependancies in adjacent tiles
[CreateAssetMenu(fileName = "New Building", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : ScriptableObject
{
    [SerializeField] private string buildingName = null;
    [SerializeField] private GameObject buildingObject = null;
    [SerializeField] private ResourceRequirement[] resourceRequirements = null;

    [SerializeField] private int CitizensToSpawn = 0;
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
    public ResourceRequirement[] GetResourceRequirements()
    {
        return resourceRequirements;
    }
    public int GetCitizensToSpawn()
    {
        return CitizensToSpawn;
    }
}
