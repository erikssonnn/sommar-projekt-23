using UnityEngine;

//Scriptable object for the building. Containing data like prefab object, resources needed, job type, dependancies in adjacent tiles
namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Building", menuName = "ScriptableObjects/Building", order = 1)]
    public class Building : ScriptableObject
    {
        public new string name;
        public GameObject obj;
        public ResourceClass resourceRequirements;
        public int citizenToSpawn;
        
        // TODO: Add job type 
        // TODO: Add dependancies in adjacent tiles
    }
}
