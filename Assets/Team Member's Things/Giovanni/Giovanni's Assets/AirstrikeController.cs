using UnityEngine;
using System.Collections;

public class AirstrikeController : MonoBehaviour
{
    [Header("Plane Settings")]
    public GameObject planeModel; 
    public float flySpeed = 150f; 

    [Header("Explosion Settings")]
    public GameObject[] explosionPrefabs; 
    public float baseExplosionRadius = 8f;
    public float extraRadiusPerUpgrade = 4f; 
    public int baseDamage = 150; 

    private Vector3 targetPosition;
    private Vector3 startPos;
    private Vector3 endPos;
    private int currentUpgradeLevel = 0;

    public void InitializeStrike(Vector3 groundTarget, int upgradeLevel)
    {
        targetPosition = groundTarget;
        currentUpgradeLevel = upgradeLevel;
        
        startPos = targetPosition + new Vector3(150f, 20f, 0f);
        endPos = targetPosition + new Vector3(-150f, 20f, 0f);
        
        planeModel.transform.position = startPos;
        planeModel.transform.LookAt(endPos);
        
        StartCoroutine(ExecuteFlyover());
    }

    IEnumerator ExecuteFlyover()
    {
        bool bombsDropped = false;

        while (Vector3.Distance(planeModel.transform.position, endPos) > 5f)
        {
            planeModel.transform.position = Vector3.MoveTowards(planeModel.transform.position, endPos, flySpeed * Time.deltaTime);

            Vector3 planeXZ = new Vector3(planeModel.transform.position.x, 0, planeModel.transform.position.z);
            Vector3 targetXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
            
            if (!bombsDropped && Vector3.Distance(planeXZ, targetXZ) < 20f)
            {
                bombsDropped = true;
                StartCoroutine(DetonateBombs()); 
            }

            yield return null; 
        }

        Destroy(gameObject); 
    }

    IEnumerator DetonateBombs()
    {
        Vector3[] offsets = new Vector3[] { 
            new Vector3(8f, 0, 0f),  
            Vector3.zero,            
            new Vector3(-8f, 0, 0f)  
        };

        GameObject selectedExplosion = explosionPrefabs[0]; 
        if (currentUpgradeLevel < explosionPrefabs.Length)
        {
            selectedExplosion = explosionPrefabs[currentUpgradeLevel];
        }

        int totalDamage = baseDamage + (currentUpgradeLevel * 50);
        float totalRadius = baseExplosionRadius + (currentUpgradeLevel * extraRadiusPerUpgrade);

        for (int i = 0; i < 3; i++)
        {
            Vector3 blastPos = targetPosition + offsets[i];
            
            if (selectedExplosion != null)
            {
                GameObject blast = Instantiate(selectedExplosion, blastPos, Quaternion.identity);
                blast.transform.localScale = Vector3.one * (totalRadius / baseExplosionRadius);
            }
            
            Collider[] hitColliders = Physics.OverlapSphere(blastPos, totalRadius);
            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    hit.SendMessage("TakeDamage", totalDamage, SendMessageOptions.DontRequireReceiver);
                }
            }
            
            yield return new WaitForSeconds(0.25f); 
        }
    }
}