using UnityEngine;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Building buildingType = null;
    public void CallSelectedBuilding()
    {
        BuildingManager.Instance.SelectBuilding(buildingType);
    }
}
