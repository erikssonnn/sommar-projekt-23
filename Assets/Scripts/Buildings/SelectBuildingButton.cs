using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBuildingButton : MonoBehaviour
{
    [SerializeField] private Building BuildingType = null;
    public void CallSelectedBuilding()
    {
        BuildingManager.Instance.SelectedBuilding(BuildingType);
    }
}
