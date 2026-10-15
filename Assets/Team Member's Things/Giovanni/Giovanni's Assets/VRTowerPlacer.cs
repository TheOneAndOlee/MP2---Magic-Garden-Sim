using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class VRTowerPlacer : MonoBehaviour
{
    [Header("Linking")]
    public StoreItemManager linkedStoreManager;
    public GameObject targetFullSizeTower; 
    public Transform originalMenuSpot;

    [Header("Placement Settings")]
    public LayerMask placementPlaneLayer; 

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool hasBeenPlaced = false;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectExited.AddListener(OnDrop);
    }

    void OnDrop(SelectExitEventArgs args)
    {
        if (hasBeenPlaced) return; 

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, placementPlaneLayer))
        {
            if (GameManager.Instance.SpendCurrency(linkedStoreManager.purchaseCost))
            {
                if (targetFullSizeTower != null)
                {
                    targetFullSizeTower.SetActive(true);
                }
                
                linkedStoreManager.NotifyTowerPlaced();

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.IncreaseAlienSpawns();
                }

                hasBeenPlaced = true;
                
                gameObject.SetActive(false);
                return; 
            }
        }

        transform.position = originalMenuSpot.position;
        transform.rotation = originalMenuSpot.rotation;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnDrop);
        }
    }
}