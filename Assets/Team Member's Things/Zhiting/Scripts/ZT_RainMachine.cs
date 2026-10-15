using System.Collections.Generic;
using UnityEngine;

public class ZT_RainMachine : MonoBehaviour
{
    //stat: storedRain
    public float storedRain = 50f;
    public float maxStoredRain = 100f;
    public float collectRate = 2f;      

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


    void Start()
    {
        grid = FindFirstObjectByType<GridManager>();
        cell = grid.WorldToCell(transform.position);

        fullLevelScale = waterLevel.localScale;

        rainCurtain.SetActive(false);
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

    void WaterNeighbors()
    {
        List<GameObject> neighbors = grid.GetNeighbors(cell);


        foreach (GameObject n in neighbors)
        {
            ZT_Bed bed = n.GetComponent<ZT_Bed>();
            if (bed != null && storedRain >= waterPerBed)
            {
                bed.moisture += waterPerBed;
                bed.moisture = Mathf.Min(bed.moisture, bed.maxMoisture);
                storedRain -= waterPerBed;
                rainTimer = rainDuration;
                watered = true;
                rainCurtain.SetActive(true);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        ZT_WaterDrop waterDrop = collision.gameObject.GetComponent<ZT_WaterDrop>();
        if (waterDrop != null)
        {
            storedRain += waterDrop.waterAmount;
            storedRain = Mathf.Min(storedRain, maxStoredRain);
            Destroy(collision.gameObject);
            Debug.Log("collect rain: " + waterDrop.waterAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ZT_WateringCan wateringCan = other.gameObject.GetComponentInParent<ZT_WateringCan> ();
        Debug.Log("Trigger entered by: " + other.gameObject.name);
        if (wateringCan != null && wateringCan.collectRain)
        {
            ZT_Resources.Instance.AddResources(ZT_ResourceType.Rainwater, storedRain);
            storedRain = 0f;
        }
    }

}