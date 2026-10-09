using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZT_GridSnap : MonoBehaviour
{
    public GridManager grid;
    public Transform target;
    public GameObject buildingPrefab;
   
    void Update()
    {
       Vector2Int cell = grid.WorldToCell(target.position);
       Vector3 position = grid.CellToWorld(cell);
        transform.position = position;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (grid.IsFree(cell))
            {
               GameObject building =  Instantiate(buildingPrefab, position, Quaternion.identity);
                grid.Register(cell, building);

            }

        }

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            List<GameObject> list = grid.GetNeighbors(cell);

            Debug.Log("Neighbor count：" + list.Count);
        }
    }
}
