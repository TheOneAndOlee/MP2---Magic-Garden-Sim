using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ZT_Flower : MonoBehaviour
{
    //stat:bloom
    public float bloom = 0f;
    public float maxBloom = 100f;
    public float growthRate = 10f;

    public Color grayColor = Color.gray;
    private Color fullColor;
    public string colorProperty = "_RColor";

    private Vector3 fullScale;

    public Color[] palette;
    private Renderer[] rends;

    //stat:water
    public float water = 5f;
    public float maxWater = 100f;
    public float waterLossRate = 1f;

    //stat:vitality
    public float vitality = 100f;
    public float maxVitality = 100f;
    public float witherRate = 2f;
    public GameObject witheredPrefab;

    public float recoverRate = 25f;

    public GameObject dreamRibbon;
    private bool harvested = false;

    //event
    public static event Action<ZT_Flower> Bloomed;
    public static event Action<ZT_Flower> Stalled;
    public static event Action<ZT_Flower> RainedOn;
    private bool broadCastBloomed = false;
    private bool broadCastStalled = false;

    //tutorial
    private bool tutorialWatered = false;


    void Start()
    {
        //bloom
        fullScale = transform.localScale;
        rends = GetComponentsInChildren<Renderer>();
        if (palette.Length > 0)
        {
            fullColor = palette[Random.Range(0, palette.Length)];
        }
        else
        {
            fullColor = rends[0].material.GetColor(colorProperty);
        }
        dreamRibbon.SetActive(false);
    }

    void Update()
    {
        if (!tutorialWatered)
        {
            water -= waterLossRate * Time.deltaTime;
        }
        water = Mathf.Max(water, 0);
        if (water > 0)
        {
            broadCastStalled = false;
            if (bloom < maxBloom)
            {
                bloom += growthRate * Time.deltaTime;
                bloom = Mathf.Min(bloom, maxBloom);
                if(bloom >= maxBloom && !broadCastBloomed)
                {
                    broadCastBloomed = true;
                    Bloomed?.Invoke(this);
                }
            }
            vitality += recoverRate * Time.deltaTime;
            vitality = Mathf.Min(vitality, maxVitality);
        }
        else 
        {
            if (bloom < maxBloom && !broadCastStalled)
            {
                broadCastStalled = true;
                Stalled?.Invoke(this);
            }
            if (ZT_ProgressManager.Current == null || ZT_ProgressManager.Current.HasReached(ZT_Stage.BuildTutorial))
            {
                vitality -= witherRate * Time.deltaTime;
                vitality = Mathf.Max(vitality, 0);
            }
        }
        float t = bloom / maxBloom;
        float colorT = t * (vitality / maxVitality);
        transform.localScale = fullScale * t;
        Color currentColor = Color.Lerp(grayColor, fullColor, colorT);
        foreach (Renderer r in rends)
        {
            r.material.SetColor(colorProperty, currentColor);
        }
        if (vitality == 0)
        {
            Instantiate(witheredPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        dreamRibbon.SetActive(bloom >= maxBloom && vitality > maxVitality * 0.5f);


    }
    void OnCollisionEnter(Collision collision)
    {
        
        ZT_WaterDrop drop = collision.gameObject.GetComponent<ZT_WaterDrop>();
        if (drop != null)
        {
            if (drop.isRain)
            {
                RainedOn?.Invoke(this);
            }
            if (ZT_ProgressManager.Current != null && !ZT_ProgressManager.Current.HasReached(ZT_Stage.BuildTutorial))
            {
                tutorialWatered = true;
            }
            water += drop.waterAmount;
            Destroy(collision.gameObject);
        }
        water = Mathf.Min(water, maxWater);
    }

    //harvest
        void OnTriggerEnter(Collider other)
        {
            ZT_Shears shears = other.gameObject.GetComponent<ZT_Shears>();
            if (bloom >= maxBloom && shears != null)
            {
                if (harvested) return;
                harvested = true;
                int seedNumber = Random.Range(1, 3);
                    ZT_Resources.Instance.AddResources(ZT_ResourceType.Flower, 1);
                    ZT_Resources.Instance.AddResources(ZT_ResourceType.Seed, seedNumber);
                if ((ZT_ProgressManager.Current != null))
                {
                    ZT_ProgressManager.Current.ReportHarvest(1);
                }
                Destroy(gameObject);

            }
            else if (shears != null)
            {
                ZT_Message.Show("This dream is not finished yet. Wait until the flower is fully bloomed and glowing.");
            }
    }
}
