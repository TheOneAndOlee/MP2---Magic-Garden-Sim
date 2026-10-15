using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
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
        
        Destroy(gameObject, 5f); 
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
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