using UnityEngine;

public class AircraftShooter : MonoBehaviour
{
    [Header("Dependencies")]
    public StoreItemManager storeManager; 
    public GameObject lineBulletPrefab;
    public Transform firePoint; 

    [Header("Combat Stats")]
    public float range = 15f;
    public float fireRate = 2f; 
    public int baseDamage = 10;
    public int damagePerUpgrade = 10;
    private float fireCountdown = 0f;
    public string enemyTag = "Enemy"; 

    [Header("Upgrade Visuals")]
    [Tooltip("Slot 0 = Base Level. Slots 1-3 = Upgrades")]
    public Material[] upgradeBulletMaterials; 

    [Header("Audio Settings")]
    public AudioClip shootSound;

    private Transform target;

    void Update()
    {
        UpdateTarget();

        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(lineBulletPrefab, firePoint.position, firePoint.rotation);
        LinearProjectile projectile = bulletGO.GetComponent<LinearProjectile>();
        
        if (projectile != null)
        {
            int level = storeManager != null ? storeManager.currentUpgradeLevel : 0;
            int safeIndex = Mathf.Clamp(level, 0, upgradeBulletMaterials.Length - 1);
            
            int calculatedDamage = baseDamage + (level * damagePerUpgrade);
            
            projectile.Initialize(target, upgradeBulletMaterials[safeIndex], calculatedDamage);
        }

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, firePoint.position);
        }
    }
}