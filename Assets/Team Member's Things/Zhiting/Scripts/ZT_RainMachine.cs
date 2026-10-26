using System;
using System.Collections.Generic;
using UnityEngine;

public class ZT_RainMachine : MonoBehaviour
{
    //stat: storedRain
    public float storedRain = 0f;
    public float maxStoredRain = 100f;
    public float collectRate = 0f;      

    // neighbor watering
    public float waterInterval = 5f;    
    public float waterPerBed = 5f;     
    private float timer;

    // visual storedRain amount
    public Transform waterLevel;
    private Vector3 fullLevelScale;

    // grid
    private GridManager grid;
    private Vector2Int cell;

    //rain curtain
    public GameObject rainCurtain;
    private bool watered = false;
    public float rainDuration = 2f;
    private float rainTimer;

    public static event Action<ZT_RainMachine> Placed;
    public static event Action<ZT_RainMachine> RainCollected;

    void Start()
    {
        //grid = FindAnyObjectByType<GridManager>();
        //cell = grid.WorldToCell(transform.position);

        fullLevelScale = waterLevel.localScale;

        rainCurtain.SetActive(false);
        Placed?.Invoke(this);
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

        // visualize rain curtain for a while
        if(watered)
        {
            rainTimer -= Time.deltaTime;
            if (rainTimer <= 0)
            {
                rainCurtain.SetActive(false);
                watered = false;
            }

        }

        // visualizing storedRain
        float t = storedRain / maxStoredRain;
        waterLevel.localScale = new Vector3(fullLevelScale.x, fullLevelScale.y * t, fullLevelScale.z);
    }

    public void SetGrid(GridManager g, Vector2Int c)
    {
        grid = g;
        cell = c;
    }
    void WaterNeighbors()
    {
        if (grid == null) return;
        List<GameObject> neighbors = grid.GetNeighbors(cell);
        int bedCount = 0;
        int wateredCount = 0;

        foreach (GameObject n in neighbors)
        {
            ZT_Bed bed = n.GetComponentInChildren<ZT_Bed>();
            if (bed == null) continue;
            bedCount++;

            if (storedRain > 0)
            {
                float need = bed.maxMoisture - bed.moisture;
                float amount = Mathf.Min(waterPerBed, need, storedRain);
                if (amount > 0.01f)
                {
                    bed.moisture += amount;
                    storedRain -= amount;
                    rainTimer = rainDuration;
                    watered = true;
                    rainCurtain.SetActive(true);
                    wateredCount++;
                }
            }
        }

        if (bedCount == 4 && wateredCount > 0 && ZT_ProgressManager.Current != null)
        {
            ZT_ProgressManager.Current.ReportFullRainMachine();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ZT_WaterDrop waterDrop = collision.gameObject.GetComponent<ZT_WaterDrop>();
        if (waterDrop != null)
        {
            storedRain += waterDrop.waterAmount;
            storedRain = Mathf.Min(storedRain, maxStoredRain);
            if (waterDrop.isRain)
            {
                RainCollected?.Invoke(this);
            }
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ZT_WateringCan wateringCan = other.gameObject.GetComponentInParent<ZT_WateringCan> ();
        if (wateringCan != null && wateringCan.collectRain)
        {
            ZT_Resources.Instance.AddResources(ZT_ResourceType.Rainwater, storedRain);
            storedRain = 0f;
        }
    }

}