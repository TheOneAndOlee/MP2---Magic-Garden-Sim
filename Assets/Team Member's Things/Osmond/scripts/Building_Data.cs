using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Building_Data", menuName = "Scriptable Objects/Building Data Object")]
public class Building_Data : ScriptableObject
{
    #region Building Costs and Stat Modifiers

    [System.Serializable]
    public struct BuildingCost
    {
        public SimResource.ResourceType resource;
        public float cost;
    }

    [System.Serializable]
    public struct StatModifier
    {
        public SimResource.StatType stat;
        public float amount;
    }

    #endregion

    #region Building Information

    public string buildingName;
    public string description;
    public GameObject buildingPrefab;
    public List<BuildingCost> buildingCosts = new List<BuildingCost>();
    public Image menuImage;
    public List<StatModifier> statModifiers = new List<StatModifier>();

    #endregion
}
