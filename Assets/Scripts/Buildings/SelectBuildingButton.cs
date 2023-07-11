using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Building buildingType = null;
    public void CallSelectedBuilding()
    {
        BuildingManager.Instance.SelectBuilding(buildingType);
    }
}
