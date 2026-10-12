using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [System.NonSerialized] public Dictionary<SimResource.ResourceType, float> Inventory = new Dictionary<SimResource.ResourceType, float>();

    public bool CanAfford(Building_Data building)
    {
        // A variable to track what resources are missing without returning early (and therefore not printing ALL the resources the player doesn't have enough of)
        bool canBuild = true; 
        foreach (Building_Data.BuildingCost bc in building.buildingCosts)
        {
            if (!Inventory.TryGetValue(bc.resource, out float cost))
            {
                Debug.Log("Player does not have " + bc.resource + " in inventory. It is required to build " + building.name);
                canBuild = false;
            }

            float surplus = Inventory[bc.resource] - cost;            
            
            if (surplus < 0)
            {
                surplus *= -1;
                Debug.Log("Player does not have enough " + bc.resource + " to build " + building.name + ". They are missing " + surplus + " " + bc.resource);
                canBuild = false;
            }
        }

        // Building can be built, subtract resources
        if (canBuild)
        {
            Debug.Log("Player has enough resources to build " + building.name);
            foreach (Building_Data.BuildingCost bc in building.buildingCosts)
            {
                // Doesn't need to be rounded, that's only done visually
                Inventory[bc.resource] -= bc.cost;
            }
            return true;
        }

        // Player can't build, don't subtract any resources
        return false;
    }
}
