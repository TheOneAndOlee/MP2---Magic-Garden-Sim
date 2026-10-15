using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class LaserPointerTargeter : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform firePoint; 
    public LineRenderer laserLine;
    public Transform mainBase; 

    [Header("Strike Prefabs")]
    public GameObject radialPulsePrefab;
    public GameObject airstrikeManagerPrefab;

    [Header("Targeting Rules")]
    public float minSafeDistance = 15f; 
    public AudioClip errorSound;
    private AudioSource audioSource;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // Maps to the VR controller trigger being pulled while the object is held
        grabInteractable.activated.AddListener(OnTriggerPulled);
    }

    void Update()
    {
        if (grabInteractable.isSelected)
        {
            laserLine.enabled = true;
            laserLine.SetPosition(0, firePoint.position);
            
            if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, 200f))
            {
                laserLine.SetPosition(1, hit.point);
            }
            else
            {
                laserLine.SetPosition(1, firePoint.position + firePoint.forward * 200f);
            }
        }
        else
        {
            laserLine.enabled = false;
        }
    }

    void OnTriggerPulled(ActivateEventArgs args)
    {
        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, 200f))
        {
            // 1. Safety Check: Is it too close to the base?
            if (mainBase != null && Vector3.Distance(hit.point, mainBase.position) < minSafeDistance)
            {
                if (errorSound != null) audioSource.PlayOneShot(errorSound);
                return;
            }

            // 2. Valid Target: Spawn the pulse and the strike
            if (radialPulsePrefab != null)
            {
                Instantiate(radialPulsePrefab, hit.point, Quaternion.identity);
            }
            
            if (airstrikeManagerPrefab != null)
            {
                GameObject strike = Instantiate(airstrikeManagerPrefab, hit.point, Quaternion.identity);
                strike.GetComponent<AirstrikeController>().InitializeStrike(hit.point);
            }
            
            // 3. Consume the item
            Destroy(gameObject); 
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(OnTriggerPulled);
        }
    }
}