using UnityEngine;

public class AlienBehavior : MonoBehaviour
{
    private Transform mainBase;
    private Transform currentTarget;
    
    [Header("Movement Settings")]
    public float minSpeed = 2f;
    public float maxSpeed = 4f;
    private float currentSpeed;

    [Header("Size Randomization")]
    public float minSize = 0.7f;
    public float maxSize = 1.4f;

    [Header("Attack Stats")]
    public float attackDamage = 10f;
    public float attackRate = 1f; 
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    [Header("Stats")]
    public float health = 100f; 
    public int killReward = 100; 

    public void SetMainBase(Transform baseTransform)
    {
        mainBase = baseTransform;
        FindNewTarget();
    }

    public void InitializeStats(int difficultyTier)
    {
        float randomScale = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        // Calculate the base random speed first
        float baseSpeed = Random.Range(minSpeed, maxSpeed);

        // Apply the exponential speed boost from the GameManager
        if (GameManager.Instance != null)
        {
            currentSpeed = baseSpeed * GameManager.Instance.alienSpeedMultiplier;
        }
        else
        {
            currentSpeed = baseSpeed;
        }

        float maxHealth = 100f + (difficultyTier * 50f);
        if (maxHealth > 250f) maxHealth = 250f; 
        health = maxHealth;
    }

    void FindNewTarget()
    {
        GameObject[] barricades = GameObject.FindGameObjectsWithTag("Defense");
        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject barricade in barricades)
        {
            float distance = Vector3.Distance(transform.position, barricade.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = barricade.transform;
            }
        }

        if (closest != null)
        {
            currentTarget = closest;
        }
        else
        {
            currentTarget = mainBase;
        }
    }

    void Update()
    {
        if (currentTarget == null)
        {
            isAttacking = false;
            FindNewTarget(); 
        }

        if (currentTarget != null && !isAttacking)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, currentSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
        }
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("Alien is touching: " + other.gameObject.name + " with tag: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Defense"))
        {
            isAttacking = true; 
            
            if (Time.time >= nextAttackTime)
            {
                Barricade barricade = other.gameObject.GetComponent<Barricade>();
                
                if (barricade != null)
                {
                    Debug.Log("Barricade found! Subtracting health.");
                    barricade.health -= (int)attackDamage; 
                }
                else
                {
                    Debug.LogWarning("Touched a Defense object, but it has no Barricade script attached!");
                }
                
                nextAttackTime = Time.time + attackRate; 
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Defense"))
        {
            isAttacking = false; 
            FindNewTarget(); 
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCurrency(killReward);
            }
            
            Destroy(gameObject);
        }
    }
}