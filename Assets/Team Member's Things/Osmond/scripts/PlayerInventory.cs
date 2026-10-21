using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    #region Variables and their Getters & Setters

    [NonSerialized]
    public Dictionary<SimResource.ResourceType, float> Inventory = new Dictionary<SimResource.ResourceType, float>();

    [Header("Starting Inventory")]
    [SerializeField] private List<Building_Data.BuildingCost> startingResources = new List<Building_Data.BuildingCost>();
    [SerializeField] private List<Building_Data> availableBuildings = new List<Building_Data>();

    [Header("Default Research Stats")]
    [SerializeField, Min(1)] private float baseMaximumResearchProgress = 1000f;
    [SerializeField, Min(1)] private float baseMaximumResearchPoints = 100f;
    [SerializeField, Min(1)] private float baseResearchPointThreshold = 100f;
    [SerializeField, Min(0)] private float baseResearchConversionFunds = 25f;

    // A HashSet prevents the same building from adding its bonuses more than once.
    private HashSet<StatBuilding> buildings = new HashSet<StatBuilding>();

    public IReadOnlyList<Building_Data> AvailableBuildings
    {
        get => availableBuildings;
    }

    public float ResearchOutput
    {
        get
        {
            float bonus = GetModifier(SimResource.StatType.ResearchOutput);
            return Mathf.Max(0f, bonus);
        }
    }

    public float ResearchFundsOutput
    {
        get
        {
            float bonus = GetModifier(SimResource.StatType.ResearchFundsOutput);
            return Mathf.Max(0f, bonus);
        }
    }

    public float MaximumResearchProgress
    {
        get
        {
            float bonus = GetModifier(SimResource.StatType.MaximumResearchProgress);
            return Mathf.Max(1f, baseMaximumResearchProgress + bonus);
        }
    }

    public float MaximumResearchPoints
    {
        get
        {
            float bonus = GetModifier(SimResource.StatType.MaximumResearchPoints);
            return Mathf.Max(1f, baseMaximumResearchPoints + bonus);
        }
    }

    public float ResearchPointThreshold
    {
        get
        {
            float reduction = GetModifier(SimResource.StatType.ResearchPointThreshold);
            return Mathf.Max(1f, baseResearchPointThreshold + reduction);
        }
    }

    public float ResearchConversionFunds
    {
        get
        {
            float reduction = GetModifier(SimResource.StatType.ResearchConversionFunds);
            return Mathf.Max(0f, baseResearchConversionFunds + reduction);
        }
    }

    public float ResearchEfficiency
    {
        get
        {
            float bonus = GetModifier(SimResource.StatType.ResearchEfficiency);
            return Mathf.Max(0f, 100f + bonus);
        }
    }

    public float Morale
    {
        get
        {
            float currentMorale = GetAmount(SimResource.ResourceType.Morale);
            float bonus = GetModifier(SimResource.StatType.Morale);
            return Mathf.Max(0f, currentMorale + bonus);
        }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        foreach (SimResource.ResourceType resource in Enum.GetValues(typeof(SimResource.ResourceType)))
        {
            if (!Inventory.ContainsKey(resource))
            {
                Inventory.Add(resource, 0f);
            }
        }

        foreach (Building_Data.BuildingCost resource in startingResources)
        {
            AddResource(resource.resource, resource.cost);
        }
    }

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    #endregion

    #region Research and Resource Methods

    public void Tick(float seconds)
    {
        float researchPerSecond = ResearchOutput * ResearchEfficiency * 0.01f;
        float researchGained = researchPerSecond * Mathf.Max(0f, seconds);
        AddResource(SimResource.ResourceType.ResearchProgressPoints, researchGained);

        float fundsGained = ResearchFundsOutput * Mathf.Max(0f, seconds);
        AddResource(SimResource.ResourceType.ResearchFunds, fundsGained);

        // Losing a storage building can lower the maximum below the current balance.
        float currentPoints = GetAmount(SimResource.ResourceType.ResearchPoints);
        Inventory[SimResource.ResourceType.ResearchPoints] = Mathf.Min(currentPoints, MaximumResearchPoints);
    }

    public float GetAmount(SimResource.ResourceType resource)
    {
        if (Inventory.TryGetValue(resource, out float amount))
        {
            return amount;
        }

        return 0f;
    }

    public void AddResource(SimResource.ResourceType resource, float amount)
    {
        float maximumAmount = float.MaxValue;

        if (resource == SimResource.ResourceType.ResearchProgressPoints)
        {
            maximumAmount = MaximumResearchProgress;
        }
        else if (resource == SimResource.ResourceType.ResearchPoints)
        {
            maximumAmount = MaximumResearchPoints;
        }

        float newAmount = GetAmount(resource) + amount;
        Inventory[resource] = Mathf.Clamp(newAmount, 0f, maximumAmount);
    }

    public bool TryConvertResearch()
    {
        float currentProgress = GetAmount(SimResource.ResourceType.ResearchProgressPoints);
        float currentFunds = GetAmount(SimResource.ResourceType.ResearchFunds);
        float requiredProgress = ResearchPointThreshold;
        float requiredFunds = ResearchConversionFunds;

        if (currentProgress < requiredProgress || currentFunds < requiredFunds)
        {
            return false;
        }

        if (GetAmount(SimResource.ResourceType.ResearchPoints) + 1f > MaximumResearchPoints)
        {
            return false;
        }

        Inventory[SimResource.ResourceType.ResearchProgressPoints] = currentProgress - requiredProgress;
        Inventory[SimResource.ResourceType.ResearchFunds] = currentFunds - requiredFunds;
        AddResource(SimResource.ResourceType.ResearchPoints, 1f);

        return true;
    }

    #endregion

    #region Building Stat Methods

    public bool IsAvailable(Building_Data data)
    {
        return data != null && availableBuildings.Contains(data);
    }

    public void Register(StatBuilding building)
    {
        if (building != null)
        {
            buildings.Add(building);
        }
    }

    public void Unregister(StatBuilding building)
    {
        if (building != null)
        {
            buildings.Remove(building);
        }
    }

    public float GetModifier(SimResource.StatType stat)
    {
        float totalBonus = 0f;

        foreach (StatBuilding building in buildings)
        {
            if (building != null)
            {
                totalBonus += building.GetModifier(stat);
            }
        }

        return totalBonus;
    }

    #endregion

    #region Building Purchase Methods

    private bool TryGetCosts(Building_Data data, out Dictionary<SimResource.ResourceType, float> costs)
    {
        costs = new Dictionary<SimResource.ResourceType, float>();

        if (data == null)
        {
            return false;
        }

        if (data.buildingCosts == null)
        {
            return true;
        }

        foreach (Building_Data.BuildingCost buildingCost in data.buildingCosts)
        {
            if (buildingCost.cost < 0f || float.IsNaN(buildingCost.cost) || float.IsInfinity(buildingCost.cost))
            {
                return false;
            }

            // Combine costs if the same resource appears more than once in the asset.
            costs.TryGetValue(buildingCost.resource, out float currentCost);
            costs[buildingCost.resource] = currentCost + buildingCost.cost;
        }

        return true;
    }

    public bool CanAfford(Building_Data data)
    {
        if (!TryGetCosts(data, out Dictionary<SimResource.ResourceType, float> costs))
        {
            return false;
        }

        foreach (var resourceCost in costs)
        {
            if (GetAmount(resourceCost.Key) < resourceCost.Value)
            {
                return false;
            }
        }

        // This only checks the balance. Opening the menu should not spend anything.
        return true;
    }

    public bool TrySpend(Building_Data data)
    {
        if (!CanAfford(data))
        {
            return false;
        }

        if (!TryGetCosts(data, out Dictionary<SimResource.ResourceType, float> costs))
        {
            return false;
        }

        foreach (var resourceCost in costs)
        {
            Inventory[resourceCost.Key] = GetAmount(resourceCost.Key) - resourceCost.Value;
        }

        return true;
    }

    public void Refund(Building_Data data)
    {
        if (!TryGetCosts(data, out Dictionary<SimResource.ResourceType, float> costs))
        {
            return;
        }

        foreach (var resourceCost in costs)
        {
            Inventory[resourceCost.Key] = GetAmount(resourceCost.Key) + resourceCost.Value;
        }
    }

    #endregion
}
