using UnityEngine;

public class ZT_RainCloud : MonoBehaviour
{
    public GameObject dropPrefab;      
    public float dropInterval = 0.1f;
    public float radius = 1.5f;         

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= dropInterval)
        {
            timer = 0;

            Vector2 offset = Random.insideUnitCircle * radius;
            Vector3 position = transform.position + new Vector3(offset.x, -0.5f, offset.y);

            Instantiate(dropPrefab, position, Quaternion.identity);
        }
    }
}