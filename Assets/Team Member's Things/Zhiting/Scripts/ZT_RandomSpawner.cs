using UnityEngine;

// 在一个矩形范围内随机生成物体。
// - Wild Instances：startCount > 0，游戏开始时生成一批
// - Intermittent Instances：勾选 intermittent，游戏中每隔随机时间生成一个
public class ZT_RandomSpawner : MonoBehaviour
{
    public GameObject[] prefabs;            
    public Vector2 areaSize = new Vector2(10f, 10f);  
    public float spawnHeight = 0f;         

    [Header("Wild: spawn at start")]
    public int startCount = 5;

    [Header("Intermittent: spawn during the game")]
    public bool intermittent = false;
    public float minInterval = 15f;
    public float maxInterval = 30f;
    public float lifetime = 0f;             

    private float timer;
    private float nextInterval;

    void Start()
    {
        for (int i = 0; i < startCount; i++)
        {
            SpawnOne();
        }

        nextInterval = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        if (!intermittent) return;

        timer += Time.deltaTime;
        if (timer >= nextInterval)
        {
            timer = 0;
            nextInterval = Random.Range(minInterval, maxInterval); 
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

        float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float z = Random.Range(-areaSize.y / 2, areaSize.y / 2);

        Vector3 position = transform.position + new Vector3(x, spawnHeight, z);

        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        GameObject obj = Instantiate(prefab, position, rotation);

        if (lifetime > 0)
        {
            Destroy(obj, lifetime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + Vector3.up * spawnHeight, new Vector3(areaSize.x, 0.1f, areaSize.y));
    }
}