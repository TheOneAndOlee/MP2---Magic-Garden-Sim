using System.Collections.Generic;
using UnityEngine;

public enum ZT_ResourceType { Flower, Seed, Rainwater, Graduate }
public class ZT_Resources : MonoBehaviour
{
    Dictionary<ZT_ResourceType, float> resources = new Dictionary<ZT_ResourceType, float>();
    public static ZT_Resources Instance;

    void Start()
    {
        AddResources(ZT_ResourceType.Seed, 20);
        AddResources(ZT_ResourceType.Rainwater, 5);
    }
    
    public float GetResources(ZT_ResourceType type)
    {
        resources.TryGetValue(type, out float amount);
        return amount;
    }

    public void AddResources(ZT_ResourceType type, float amount)
    {
        float existingAmount = GetResources(type);
        resources[type] = existingAmount + amount;
        Debug.Log(type + ": " + resources[type]);
    }

    public bool TrySpend(ZT_ResourceType type, float amount)
    {
        float existingAmount = GetResources(type);
        if (existingAmount >= amount)
        {
            resources[type] = existingAmount - amount;
            Debug.Log(type + ": " + resources[type]);
            return true;
        }
        else
        {
            return false;
        }
    }
    
    void Awake()
    {
        Instance = this;
    }

}
