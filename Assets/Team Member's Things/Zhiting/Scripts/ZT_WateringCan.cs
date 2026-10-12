using UnityEngine;

public class ZT_WateringCan : MonoBehaviour
{
    public GameObject dropPrefab;
    public Transform spout;
    public float pourAngle = 60f;
    public float dropInterval = 0.1f;

    private float timer;

    void Update()
    {
        float currentAngle = Vector3.Angle(transform.up, Vector3.up);
        if (currentAngle >= pourAngle)
        {
            timer += Time.deltaTime;
            if (timer >= dropInterval)
            {
                Instantiate(dropPrefab, spout.position, spout.rotation);
                timer = 0;
            }
        }
    }
}
