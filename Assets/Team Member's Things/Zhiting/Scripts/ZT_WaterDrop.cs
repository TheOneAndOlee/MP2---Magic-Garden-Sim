using UnityEngine;

public class ZT_WaterDrop : MonoBehaviour
{
    public float waterAmount = 10f;

    void Start()
    {
        Destroy(gameObject, 5f);
    }
}
