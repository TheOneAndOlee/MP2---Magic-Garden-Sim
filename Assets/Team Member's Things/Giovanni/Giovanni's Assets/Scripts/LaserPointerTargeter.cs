using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class LaserPointerTargeter : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform firePoint; 
    public LineRenderer laserLine;
    public Transform mainBase; 
    public GameObject previewCylinderPrefab; 
    public StoreItemManager storeManager; 
    public Transform originalMenuSpot; 

    [Header("Status Materials")]
    public Material redFadedMat;
    public Material greenFadedMat;
    public Material orangeSolidMat;

    [Header("Strike Prefabs & Upgrades")]
    public GameObject airstrikeManagerPrefab;
    [HideInInspector] public int weaponUpgradeLevel = 0; 

    [Header("Targeting Rules")]
    public float minSafeDistance = 15f; 
    
    [Header("Audio Settings")]
    public AudioClip errorSound;
    public AudioClip flybySound;

    private AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private GameObject currentPreview;
    private Renderer previewRenderer;
    private bool hasBeenGrabbed = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        
        if (rb != null) rb.isKinematic = true;

        grabInteractable.activated.AddListener(OnTriggerPulled);
        grabInteractable.selectExited.AddListener(OnDrop);
    }

    void Start()
    {
        SpawnNewPreview();
    }

    void SpawnNewPreview()
    {
        if (previewCylinderPrefab != null)
        {
            currentPreview = Instantiate(previewCylinderPrefab);
            previewRenderer = currentPreview.GetComponent<Renderer>();
            currentPreview.SetActive(false);
        }
    }

    void OnDrop(SelectExitEventArgs args)
    {
        if (originalMenuSpot != null)
        {
            transform.position = originalMenuSpot.position;
            transform.rotation = originalMenuSpot.rotation;
            
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else if (hasBeenGrabbed && rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true; 
        }
    }

    void Update()
    {
        if (grabInteractable.isSelected && !hasBeenGrabbed)
        {
            hasBeenGrabbed = true; 
            
            if (rb != null) rb.isKinematic = false; 
            
            if (storeManager != null && GameManager.Instance != null)
            {
                if (GameManager.Instance.SpendCurrency(storeManager.purchaseCost))
                {
                    storeManager.NotifyTowerPlaced(); 
                    GameManager.Instance.IncreaseAlienSpawns(); 
                    Debug.Log("Weapon successfully purchased!");
                }
                else
                {
                    Debug.LogWarning("Not enough money for the base purchase!");
                }
            }
            else
            {
                Debug.LogError("CRITICAL: The Store Manager slot is empty on the Laser Pointer! The transaction failed.");
            }
        }

        if (!grabInteractable.isSelected)
        {
            if (laserLine != null) laserLine.enabled = false;
            if (currentPreview != null) currentPreview.SetActive(false);
            return;
        }

        if (laserLine != null)
        {
            laserLine.enabled = true;
            laserLine.SetPosition(0, firePoint.position);
        }
        
        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, 200f))
        {
            if (laserLine != null) laserLine.SetPosition(1, hit.point);
            
            if (currentPreview != null)
            {
                currentPreview.SetActive(true);
                currentPreview.transform.position = hit.point + new Vector3(0, 0.1f, 0); 

                if (mainBase != null && Vector3.Distance(hit.point, mainBase.position) < minSafeDistance)
                {
                    if (previewRenderer != null) previewRenderer.material = redFadedMat;
                }
                else
                {
                    if (previewRenderer != null) previewRenderer.material = greenFadedMat;
                }
            }
        }
        else
        {
            if (laserLine != null) laserLine.SetPosition(1, firePoint.position + firePoint.forward * 200f);
            if (currentPreview != null) currentPreview.SetActive(false);
        }
    }

    void OnTriggerPulled(ActivateEventArgs args)
    {
        if (currentPreview == null || !currentPreview.activeSelf) return;

        if (mainBase != null && Vector3.Distance(currentPreview.transform.position, mainBase.position) < minSafeDistance)
        {
            if (errorSound != null) audioSource.PlayOneShot(errorSound);
            return;
        }

        if (previewRenderer != null) previewRenderer.material = orangeSolidMat;
        
        GameObject lockedPreview = currentPreview;
        currentPreview = null; 
        
        if (flybySound != null)
        {
            AudioSource.PlayClipAtPoint(flybySound, lockedPreview.transform.position);
        }

        Destroy(lockedPreview, 5f); 
        
        if (airstrikeManagerPrefab != null)
        {
            GameObject strike = Instantiate(airstrikeManagerPrefab, lockedPreview.transform.position, Quaternion.identity);
            strike.GetComponent<AirstrikeController>().InitializeStrike(lockedPreview.transform.position, weaponUpgradeLevel);
        }

        SpawnNewPreview();
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(OnTriggerPulled);
            grabInteractable.selectExited.RemoveListener(OnDrop);
        }
    }
}