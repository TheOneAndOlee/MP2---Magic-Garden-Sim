using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    #region Variables

    // Kept for existing Inspector references. All placement cells are one metre wide.
    [HideInInspector] public float cellSize = 1f;

    private Dictionary<Vector2Int, GameObject> occupied = new Dictionary<Vector2Int, GameObject>();

    #endregion

    #region Building Registration

    public bool IsFree(Vector2Int cell)
    {
        return GetAt(cell) == null;
    }

    public bool TryRegister(Vector2Int cell, GameObject obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (!IsFree(cell) && GetAt(cell) != obj)
        {
            return false;
        }

        occupied[cell] = obj;
        return true;
    }

    public void Register(Vector2Int cell, GameObject obj)
    {
        TryRegister(cell, obj);
    }

    public void Unregister(Vector2Int cell)
    {
        occupied.Remove(cell);
    }

    public void Unregister(Vector2Int cell, GameObject owner)
    {
        // A building should only remove its own grid registration.
        if (GetAt(cell) == owner)
        {
            occupied.Remove(cell);
        }
    }

    public GameObject GetAt(Vector2Int cell)
    {
        occupied.TryGetValue(cell, out GameObject building);

        if (building == null)
        {
            occupied.Remove(cell);
        }

        return building;
    }

    #endregion

    #region Grid Helpers

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 gridPosition = worldPos - transform.position;
        int x = Mathf.FloorToInt(gridPosition.x);
        int z = Mathf.FloorToInt(gridPosition.z);

        return new Vector2Int(x, z);
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        // Add half a cell so the building sits in the centre of its square.
        Vector3 cellCentre = new Vector3(cell.x + 0.5f, 0f, cell.y + 0.5f);
        return transform.position + cellCentre;
    }

    public List<GameObject> GetNeighbors(Vector2Int cell)
    {
        List<GameObject> neighbors = new List<GameObject>();
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        foreach (Vector2Int direction in directions)
        {
            GameObject neighbor = GetAt(cell + direction);

            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    #endregion
}
