using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ZT_Bed : MonoBehaviour
{
    //stat: moisture
    public float moisture = 0f;
    public float maxMoisture = 100f;
    public float moistureLossRate = 1f;

    // grow flower
    public float absorbRate = 5f;


    public ZT_Flower[] flowerPrefabs;   
    public Transform plantPoint;        
    private ZT_Flower currentFlower;   

    // color
    public Color dryColor = new Color(0.55f, 0.5f, 0.45f);
    public Color wetColor = new Color(0.25f, 0.2f, 0.15f);
    private Renderer rend;

    //event
    public static event Action<ZT_Bed, ZT_Flower> FlowerPlanted;
    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
    }
    public bool HasFlower => currentFlower != null;

    void Update()
    {

        moisture -= moistureLossRate * Time.deltaTime;
        moisture = Mathf.Max(moisture, 0);

        if (currentFlower != null && moisture > 0)
        {
            moisture -= absorbRate * Time.deltaTime;
            moisture = Mathf.Max(moisture, 0);
            currentFlower.water += absorbRate * Time.deltaTime;
            currentFlower.water = Mathf.Min(currentFlower.water, currentFlower.maxWater);
        }

        float t = moisture / maxMoisture;
        rend.material.color = Color.Lerp(dryColor, wetColor, t);
    }

    void OnCollisionEnter(Collision collision)
    {
        ZT_WaterDrop drop = collision.gameObject.GetComponent<ZT_WaterDrop>();
        if (drop != null)
        {
            moisture += drop.waterAmount;
            moisture = Mathf.Min(moisture, maxMoisture);
            Destroy(collision.gameObject);
            return;
        }

        ZT_Seed seed = collision.gameObject.GetComponent<ZT_Seed>();
        if (seed != null && currentFlower == null)
        {
            currentFlower = Instantiate(flowerPrefabs[Random.Range(0, flowerPrefabs.Length)], plantPoint.position, plantPoint.rotation);
            FlowerPlanted?.Invoke(this, currentFlower);
            Destroy(collision.gameObject);
        }
    }
}