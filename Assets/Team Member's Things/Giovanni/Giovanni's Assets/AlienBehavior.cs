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

    [Header("Audio Settings")]
    public AudioClip deathSound;
    public AudioClip idleLoopSound; 
    private AudioSource loopSource;

    void Start()
    {
        if (idleLoopSound != null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.clip = idleLoopSound;
            loopSource.loop = true;
            loopSource.spatialBlend = 1f; 
            loopSource.maxDistance = 20f; 
            loopSource.rolloffMode = AudioRolloffMode.Linear;
            loopSource.Play();
        }
    }

    public void SetMainBase(Transform baseTransform)
    {
        mainBase = baseTransform;
        FindNewTarget();
    }

    public void InitializeStats(int difficultyTier)
    {
        float randomScale = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        float baseSpeed = Random.Range(minSpeed, maxSpeed);

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
            if (barricade == null) continue;

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
        if (other == null || other.gameObject == null) return;

        if (other.gameObject.CompareTag("Defense"))
        {
            isAttacking = true; 
            
            if (Time.time >= nextAttackTime)
            {
                BarrierHealth barrier = other.gameObject.GetComponent<BarrierHealth>();
                
                if (barrier != null)
                {
                    Debug.Log("BarrierHealth found! Dealing damage.");
                    barrier.TakeDamage((int)attackDamage); 
                }
                else
                {
                    Debug.LogWarning("Touched a Defense object, but it has no BarrierHealth script attached!");
                }
                
                nextAttackTime = Time.time + attackRate; 
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == null || other.gameObject == null) return;

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
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCurrency(killReward);
            }
            
            Destroy(gameObject);
        }
    }
}