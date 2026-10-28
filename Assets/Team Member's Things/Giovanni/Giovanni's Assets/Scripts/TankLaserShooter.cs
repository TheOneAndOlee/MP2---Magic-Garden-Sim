using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class TankLaserShooter : MonoBehaviour
{
    [Header("Dependencies")]
    public StoreItemManager storeManager; 
    public Transform firePoint; 
    private LineRenderer laserLine;

    [Header("Combat Stats")]
    public float range = 15f;
    public float fireRate = 1f; 
    public int baseDamage = 10;
    public int damagePerUpgrade = 10;
    private float fireCountdown = 0f;
    public string enemyTag = "Enemy"; 

    [Header("Upgrade Visuals")]
    [Tooltip("Slot 0 = Base Level. Slots 1-3 = Upgrades")]
    public Material[] upgradeLaserMaterials; 

    [Header("Audio Settings")]
    public AudioClip shootSound; 

    private Transform target;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = false;
        
        laserLine.positionCount = 2;
        laserLine.startWidth = 0.1f;
        laserLine.endWidth = 0.1f;
    }

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
            StartCoroutine(ShootLaser());
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

    IEnumerator ShootLaser()
    {
        int level = storeManager != null ? storeManager.currentUpgradeLevel : 0;
        int safeIndex = Mathf.Clamp(level, 0, upgradeLaserMaterials.Length - 1);
        int calculatedDamage = baseDamage + (level * damagePerUpgrade);

        if (upgradeLaserMaterials.Length > 0 && upgradeLaserMaterials[safeIndex] != null)
        {
            laserLine.material = upgradeLaserMaterials[safeIndex];
        }

        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, target.position);
        laserLine.enabled = true;

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, firePoint.position);
        }

        target.SendMessage("TakeDamage", calculatedDamage, SendMessageOptions.DontRequireReceiver);

        yield return new WaitForSeconds(0.1f);
        
        laserLine.enabled = false;
    }
}