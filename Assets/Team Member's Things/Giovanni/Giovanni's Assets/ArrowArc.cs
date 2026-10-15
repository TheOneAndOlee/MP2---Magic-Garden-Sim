using UnityEngine;

public class ArrowArc : MonoBehaviour
{
    [HideInInspector] public Transform target;
    
    public float speed = 15f;
    public float arcHeight = 3f;
    public float damage = 25f;

    private Vector3 startPosition;
    private float journeyTime = 0f;
    private float totalDistance;
    
    // 1. We create a variable to lock the size
    private Vector3 originalScale; 

    void Start()
    {
        startPosition = transform.position;
        
        // 2. We record the exact size of the arrow the moment it spawns
        originalScale = transform.localScale; 
        
        if (target != null) totalDistance = Vector3.Distance(startPosition, target.position);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        journeyTime += Time.deltaTime * speed;
        float percent = journeyTime / totalDistance;

        Vector3 currentPos = Vector3.Lerp(startPosition, target.position, percent);
        currentPos.y += arcHeight * Mathf.Sin(percent * Mathf.PI);
        
        // 3. We normalize the direction vector to prevent distance-based squishing
        Vector3 moveDirection = (currentPos - transform.position).normalized;
        if (moveDirection != Vector3.zero) transform.up = moveDirection;

        transform.position = currentPos;
        
        // 4. We force Unity to maintain the starting size every single frame
        transform.localScale = originalScale; 

        if (percent >= 1f)
        {
            AlienBehavior alien = target.GetComponent<AlienBehavior>();
            if (alien != null) alien.TakeDamage(damage);
            
            Destroy(gameObject);
        }
    }
}