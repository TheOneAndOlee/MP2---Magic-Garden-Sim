using UnityEngine;

[RequireComponent(typeof(Building))]
public abstract class StatBuilding : MonoBehaviour
{
    #region Variables and their Getters & Setters

    private Building building;

    public Building Building
    {
        get
        {
            if (building == null)
            {
                building = GetComponent<Building>();
            }

            return building;
        }
    }

    #endregion

    #region Stat Methods

    public abstract float GetBaseModifier(SimResource.StatType stat);

    public float GetModifier(SimResource.StatType stat)
    {
        if (!isActiveAndEnabled || !Building.IsAlive)
        {
            return 0f;
        }

        Building_Data buildingData = Building.Data;
        float modifierAmount = 0f;

        if (buildingData == null || buildingData.statModifiers == null || buildingData.statModifiers.Count == 0)
        {
            // A manually added building can use the values in its own script.
            modifierAmount = GetBaseModifier(stat);
        }
        else
        {
            foreach (Building_Data.StatModifier modifier in buildingData.statModifiers)
            {
                if (modifier.stat == stat)
                {
                    modifierAmount += modifier.amount;
                }
            }
        }

        if (this is DrinkDispenser && BuildingNeighbors.HasNeighbor<SnackVendor>(Building))
        {
            modifierAmount *= 2f;
        }

        // Negative cost/threshold bonuses also become weaker as health drops.
        return modifierAmount * Building.EfficacyMultiplier;
    }

    #endregion
}
