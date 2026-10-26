using UnityEngine;

public class ZT_WateringCan : MonoBehaviour
{
    public GameObject dropPrefab;
    public Transform spout;
    public float pourAngle = 60f;
    public float dropInterval = 0.1f;

    private float timer;

    // for resources

    [Header("Optional resource cost")]
    public bool useResource = false;
    public ZT_ResourceType resourceType;
    public float costPerDrop = 1f;    
    public bool collectRain = false;

    //error message
    [TextArea] public string emptyMessage = "Your watering can is empty.";
    private bool warnedEmpty = false;


    void Update()
    {
        float currentAngle = Vector3.Angle(transform.up, Vector3.up);
        if (currentAngle >= pourAngle)
        {
            timer += Time.deltaTime;
            if (timer >= dropInterval)
            {
                timer = 0;
                if (useResource && !ZT_Resources.Instance.TrySpend(resourceType, costPerDrop))
                {
                    if (!warnedEmpty)
                    {
                        warnedEmpty = true;
                        ZT_Message.Show(emptyMessage);
                    }
                    return;
                }
                Instantiate(dropPrefab, spout.position, spout.rotation);
            }
        }
        else
        {
            warnedEmpty = false;
        }
    }
}
