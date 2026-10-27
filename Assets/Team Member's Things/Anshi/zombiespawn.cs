using UnityEngine;

public class zombiespawn : MonoBehaviour
{
    public GameObject zombiePrefab;

    public float spawnRadius = 10f;
    public float spawnInterval = 5f;

    public int maxZombies = 20;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnZombie();
        }
    }

    void SpawnZombie()
    {
        if (GameObject.FindGameObjectsWithTag("Zombie").Length >= maxZombies)
            return;

        float randomX = Random.Range(-5f, 10f);
    float randomZ = Random.Range(-50f, -68f);

    Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ);

    Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }
}