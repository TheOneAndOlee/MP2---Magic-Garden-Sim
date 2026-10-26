using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    public GameObject alienPrefab;
    public Transform mainBase;
    
    [Header("Spawn Settings")]
    public float baseSpawnInterval = 4f;
    private float spawnTimer = 0f;

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            Spawn();
            
            // 1. Get current tier from GameManager
            int currentTier = 0;
            if (GameManager.Instance != null)
            {
                currentTier = GameManager.Instance.globalUpgradeTier;
            }
            
            // 2. Shrink interval as tiers go up (minimum 1 second)
            float currentInterval = Mathf.Max(1f, baseSpawnInterval - (currentTier * 0.6f));
            spawnTimer = currentInterval;
        }
    }

    void Spawn()
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 randomPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), transform.position.y + 1f, Random.Range(bounds.min.z, bounds.max.z));
        
        GameObject newAlien = Instantiate(alienPrefab, randomPos, Quaternion.identity);
        
        // Pass the base reference and initialize the tiered stats
        AlienBehavior behavior = newAlien.GetComponent<AlienBehavior>();
        if (behavior != null)
        {
            behavior.SetMainBase(mainBase);
            if (GameManager.Instance != null)
            {
                behavior.InitializeStats(GameManager.Instance.globalUpgradeTier);
            }
        }
    }
}