using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public float cellSize = 1;
    Dictionary<Vector2Int, GameObject> occupied = new Dictionary<Vector2Int, GameObject>();

    public bool IsFree(Vector2Int cell)
    {
        return !occupied.ContainsKey(cell);
    }

    public void Register(Vector2Int cell, GameObject obj)
    {
        occupied[cell] = obj;
    }

    public void Unregister(Vector2Int cell)
    {
        occupied.Remove(cell);
    }

    public GameObject GetAt(Vector2Int cell)
    {
       occupied.TryGetValue(cell, out var v);
       return v;
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int z = Mathf.FloorToInt(worldPos.z / cellSize);
        return new Vector2Int (x, z);
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        float x = (cell.x + 0.5f) * cellSize;
        float z = (cell.y + 0.5f) * cellSize;
        return new Vector3(x, 0, z);
    }

    public List<GameObject> GetNeighbors(Vector2Int cell)
    {
        List<GameObject> result = new List<GameObject>();
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        foreach (Vector2Int dir in directions)
        {
            GameObject obj = GetAt(cell + dir);
           if (obj != null)
            {
                result.Add(obj);
            }
        }

        return result;
    }

}
