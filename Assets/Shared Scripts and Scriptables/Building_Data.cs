using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Building_Data", menuName = "Scriptable Objects/Building Data Object")]
public class Building_Data : ScriptableObject
{
    [System.Serializable]
    public struct BuildingCost
    {
        public SimResource.ResourceType resource;
        public float cost;
    }
    
    public string buildingName;
    public string description;
    public GameObject buildingPrefab;
    public List<BuildingCost> buildingCosts;    
}
