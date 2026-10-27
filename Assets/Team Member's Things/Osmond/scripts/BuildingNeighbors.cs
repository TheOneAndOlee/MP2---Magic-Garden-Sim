using UnityEngine;

// Rules use the four touching grid cells. Diagonals and duplicate neighbors do not count.
public static class BuildingNeighbors
{
    private static readonly Vector2Int[] directions =
    {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
    };

    public static bool HasNeighbor<T>(Building building) where T : StatBuilding
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Building neighbor = GetNeighbor(building, directions[i]);
            if (neighbor != null && neighbor.GetComponent<T>() != null)
            {
                return true;
            }
        }

        return false;
    }

    public static Building GetNeighbor(Building building, Vector2Int direction)
    {
        if (building == null || building.Grid == null || !building.IsAlive || !building.isActiveAndEnabled)
        {
            return null;
        }

        if (building.Grid.GetAt(building.Cell) != building.gameObject)
        {
            return null;
        }

        GameObject neighborObject = building.Grid.GetAt(building.Cell + direction);
        if (neighborObject == null || neighborObject == building.gameObject)
        {
            return null;
        }

        Building neighbor = neighborObject.GetComponent<Building>();
        if (neighbor == null || !neighbor.IsAlive || !neighbor.isActiveAndEnabled)
        {
            return null;
        }

        return neighbor;
    }

    public static bool PreventsDecay(Building building)
    {
        if (building.GetComponent<ArcadeMachine>() != null)
        {
            return true;
        }

        if (building.GetComponent<SmallCrate>() != null && HasNeighbor<LargeCrate>(building))
        {
            return true;
        }

        if (building.GetComponent<LargeCrate>() != null && HasNeighbor<SmallCrate>(building))
        {
            return true;
        }

        if (building.GetComponent<GasCylinder>() != null && HasNeighbor<Capsule>(building))
        {
            return true;
        }

        if (building.GetComponent<Capsule>() != null && HasNeighbor<GasCylinder>(building))
        {
            return true;
        }

        if (building.GetComponent<DrinkDispenser>() != null || building.GetComponent<SnackVendor>() != null)
        {
            // Both food buildings must touch the SAME arcade. They need not touch each other.
            foreach (Vector2Int direction in directions)
            {
                Building arcade = GetNeighbor(building, direction);
                if (arcade != null && arcade.GetComponent<ArcadeMachine>() != null
                    && HasNeighbor<DrinkDispenser>(arcade) && HasNeighbor<SnackVendor>(arcade))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool BoostsResearch(Building building)
    {
        bool generatesResearch = building.GetComponent<Terminal_Building>() != null
            || building.GetComponent<SmallMonitor>() != null || building.GetComponent<ComputerPanel>() != null;
        return generatesResearch && HasNeighbor<GasCylinder>(building);
    }
}
