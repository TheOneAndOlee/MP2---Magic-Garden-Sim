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
    private GameObject witheredFlower;

    // color
    public Color dryColor = new Color(0.55f, 0.5f, 0.45f);
    public Color wetColor = new Color(0.25f, 0.2f, 0.15f);
    private Renderer rend;

    //event
    public static event Action<ZT_Bed, ZT_Flower> FlowerPlanted;
    public static event Action<ZT_Bed> RainedOn;

    public bool HasFlower => currentFlower != null || witheredFlower != null;



    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
    }

    void Update()
    {

        moisture -= moistureLossRate * Time.deltaTime;
        moisture = Mathf.Max(moisture, 0);

        if (currentFlower != null && moisture > 0)
        {
            float need = currentFlower.maxWater - currentFlower.water;
            float amount = Mathf.Min(absorbRate * Time.deltaTime, need, moisture);
            if (amount > 0)
            {
                moisture -= amount;
                currentFlower.water += amount;
            }
        }

        float t = moisture / maxMoisture;
        rend.material.color = Color.Lerp(dryColor, wetColor, t);
    }

    public void SetWithered(GameObject withered)
    {
        witheredFlower = withered;
    }


    void OnCollisionEnter(Collision collision)
    {
        ZT_WaterDrop drop = collision.gameObject.GetComponent<ZT_WaterDrop>();
        if (drop != null)
        {
            moisture += drop.waterAmount;
            moisture = Mathf.Min(moisture, maxMoisture);
            if (drop.isRain)
            {
                RainedOn?.Invoke(this);
            }
            Destroy(collision.gameObject);
            return;
        }

        ZT_Seed seed = collision.gameObject.GetComponent<ZT_Seed>();
        if (seed != null)
        {
            if (witheredFlower != null) 
            {
                ZT_Message.Show("A withered flower is still in this bed. Cut it with your sickle first, then sow a new seed.");
                seed.Refund();
            }
            else if (currentFlower == null)
            {
                currentFlower = Instantiate(flowerPrefabs[Random.Range(0, flowerPrefabs.Length)], plantPoint.position, plantPoint.rotation);
                currentFlower.bed = this;
                FlowerPlanted?.Invoke(this, currentFlower);
                seed.Consume();
            }
            else
            {
                ZT_Message.Show("This bed already has a flower. Sow your seed in an empty bed.");
                seed.Refund();
            }
            
        }
    }
}