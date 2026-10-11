using UnityEngine;

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
    public float water = 100f;
    public float waterLossRate = 5f;

    //stat:vitality
    public float vitality = 100f;
    public float maxVitality = 100f;
    public float witherRate = 5f;
    public GameObject witheredPrefab;




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
    }

    void Update()
    {
        water -= waterLossRate * Time.deltaTime;
        water = Mathf.Max(water, 0);
        if (water > 0)
        {
            if (bloom < maxBloom)
            {
                bloom += growthRate * Time.deltaTime;
                bloom = Mathf.Min(bloom, maxBloom);
            }
            vitality += witherRate * Time.deltaTime;
            vitality = Mathf.Min(vitality, maxVitality);
        }
        else
        {
            vitality -= witherRate * Time.deltaTime;
            vitality = Mathf.Max(vitality, 0);
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


    }
}
