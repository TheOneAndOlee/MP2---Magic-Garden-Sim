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

        float randomX = Random.Range(-9.8f, 9.8f);
    float randomZ = Random.Range(-9.8f, 9.8f);

    Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ);

    Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }
}