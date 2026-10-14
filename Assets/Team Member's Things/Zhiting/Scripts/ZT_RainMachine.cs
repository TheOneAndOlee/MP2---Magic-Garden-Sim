using System.Collections.Generic;
using UnityEngine;

public class ZT_RainMachine : MonoBehaviour
{
    //stat: storedRain
    public float storedRain = 50f;
    public float maxStoredRain = 100f;
    public float collectRate = 2f;      

    // neighbor watering
    public float waterInterval = 2f;    
    public float waterPerBed = 5f;     
    private float timer;

    // visual storedRain amount
    public Transform waterLevel;
    private Vector3 fullLevelScale;

    // grid
    private GridManager grid;
    private Vector2Int cell;

    void Start()
    {
        grid = FindFirstObjectByType<GridManager>();
        cell = grid.WorldToCell(transform.position);

        fullLevelScale = waterLevel.localScale;
    }

    void Update()
    {
        // collecting storedRain
        storedRain += collectRate * Time.deltaTime;
        storedRain = Mathf.Min(storedRain, maxStoredRain);

        // providing storedRain
        timer += Time.deltaTime;
        if (timer >= waterInterval)
        {
            timer = 0;
            WaterNeighbors();
        }

        // visualizing storedRain
        float t = storedRain / maxStoredRain;
        waterLevel.localScale = new Vector3(fullLevelScale.x, fullLevelScale.y * t, fullLevelScale.z);
    }

    void WaterNeighbors()
    {
        List<GameObject> neighbors = grid.GetNeighbors(cell);

        foreach (GameObject N in neighbors)
        {
            ZT_Bed bed = N.GetComponent<ZT_Bed>();
            if (bed != null && storedRain >= waterPerBed)
            {
                bed.moisture += waterPerBed;
                bed.moisture = Mathf.Min(bed.moisture, bed.maxMoisture);
                storedRain -= waterPerBed;
            }
        }

    }
}