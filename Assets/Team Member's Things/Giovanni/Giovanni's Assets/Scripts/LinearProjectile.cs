using UnityEngine;

public class LinearProjectile : MonoBehaviour
{
    public float speed = 60f; 
    public int damage = 10; 
    private Transform target;

    public void Initialize(Transform targetAlien, Material emissionMaterial, int damageAmount)
    {
        target = targetAlien;
        damage = damageAmount; 
        
        if (emissionMaterial != null)
        {
            GetComponent<Renderer>().material = emissionMaterial;
        }

        if (target != null)
        {
            transform.LookAt(target.position);
        }
        
        Destroy(gameObject, 5f); 
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.LookAt(target.position);

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(Vector3.forward * distanceThisFrame);
    }

    void HitTarget()
    {
        if (target != null)
        {
            target.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
        
        Destroy(gameObject); 
    }
}