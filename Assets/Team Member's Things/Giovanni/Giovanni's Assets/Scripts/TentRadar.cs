using UnityEngine;

public class TentRadar : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform[] floatingBows; 
    
    public float attackRadius = 15f;
    public float fireRate = 0.25f; 
    private float fireTimer;
    
    private int currentBowIndex = 0; 

    [Header("Audio Settings")]
    public AudioClip shootSound; 

    void Update()
    {
        fireTimer -= Time.deltaTime;
        
        if (fireTimer <= 0f)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return;

            Transform closestEnemy = FindClosestEnemy();
            
            if (closestEnemy != null)
            {
                Shoot(closestEnemy);
                fireTimer = fireRate;
            }
        }
    }

    void Shoot(Transform target)
    {
        // 1. Pick the bow whose turn it is
        Transform bow = floatingBows[currentBowIndex];
        
        // 2. Fire the arrow from that specific bow
        GameObject newArrow = Instantiate(arrowPrefab, bow.position, bow.rotation);
        ArrowArc arrowScript = newArrow.GetComponent<ArrowArc>();
        if (arrowScript != null) arrowScript.target = target;
        
        // Play the shooting sound at the specific bow's location
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, bow.position);
        }
        
        // 3. Move to the next bow in line. If we hit the end, loop back to the first bow.
        currentBowIndex++;
        if (currentBowIndex >= floatingBows.Length)
        {
            currentBowIndex = 0;
        }
    }

    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDistance = attackRadius; 

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }
        return closest;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}