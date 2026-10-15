using UnityEngine;
using System.Collections;

public class AirstrikeController : MonoBehaviour
{
    [Header("Plane Settings")]
    public GameObject planeModel; 
    public float flySpeed = 150f; // Very fast zoom

    [Header("Explosion Settings")]
    public GameObject explosionEffectPrefab;
    public float explosionRadius = 8f;
    public int damage = 150; 

    private Vector3 targetPosition;
    private Vector3 startPos;
    private Vector3 endPos;

    public void InitializeStrike(Vector3 groundTarget)
    {
        targetPosition = groundTarget;
        
        // Spawn the plane high in the sky, far behind the target, flying toward a point far past it
        startPos = targetPosition + new Vector3(-150f, 40f, -150f);
        endPos = targetPosition + new Vector3(150f, 40f, 150f);
        
        planeModel.transform.position = startPos;
        planeModel.transform.LookAt(endPos);
        
        StartCoroutine(ExecuteFlyover());
    }

    IEnumerator ExecuteFlyover()
    {
        bool bombsDropped = false;

        // Move the plane until it reaches its destination
        while (Vector3.Distance(planeModel.transform.position, endPos) > 5f)
        {
            planeModel.transform.position = Vector3.MoveTowards(planeModel.transform.position, endPos, flySpeed * Time.deltaTime);

            // Calculate the 2D distance to know when the plane is directly overhead
            Vector3 planeXZ = new Vector3(planeModel.transform.position.x, 0, planeModel.transform.position.z);
            Vector3 targetXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
            
            if (!bombsDropped && Vector3.Distance(planeXZ, targetXZ) < 20f)
            {
                bombsDropped = true;
                StartCoroutine(DetonateBombs()); // Start explosions without stopping the plane
            }

            yield return null; // Wait for the next frame
        }

        Destroy(gameObject); // Cleanup the manager and plane once it flies away
    }

    IEnumerator DetonateBombs()
    {
        // A 3-point line blast cutting across the target area
        Vector3[] offsets = new Vector3[] { 
            new Vector3(-4f, 0, -4f), // First hit slightly before target
            Vector3.zero,             // Second hit dead center
            new Vector3(4f, 0, 4f)    // Third hit slightly past target
        };

        for (int i = 0; i < 3; i++)
        {
            Vector3 blastPos = targetPosition + offsets[i];
            
            if (explosionEffectPrefab != null)
            {
                Instantiate(explosionEffectPrefab, blastPos, Quaternion.identity);
            }
            
            // Damage nearby aliens
            Collider[] hitColliders = Physics.OverlapSphere(blastPos, explosionRadius);
            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    hit.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                }
            }
            
            yield return new WaitForSeconds(0.25f); // The 0.25s stagger
        }
    }
}