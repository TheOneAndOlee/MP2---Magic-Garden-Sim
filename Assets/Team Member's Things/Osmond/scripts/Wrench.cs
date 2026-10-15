using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class Wrench : MonoBehaviour
{
    #region Variables

    [Tooltip("How much health each wrench hit restores")]
    [SerializeField, Min(0)]
    private float repairAmount = 100f;

    [Tooltip("Time before the same building can be repaired again")]
    [SerializeField, Min(0)]
    private float repairCooldown = 0.35f;

    [Tooltip("The player must be holding the wrench to repair a building")]
    [SerializeField]
    private bool requireHeld = true;

    private XRGrabInteractable grabInteractable;
    private Dictionary<Building, float> nextRepairTimes = new Dictionary<Building, float>();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnDisable()
    {
        nextRepairTimes.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Building building = collision.collider.GetComponentInParent<Building>();
        TryRepair(building);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Some building colliders are on a child object, such as a crate lid.
        Building building = other.GetComponentInParent<Building>();
        TryRepair(building);
    }

    #endregion

    #region Repair Methods

    public bool TryRepair(Building building)
    {
        if (building == null || !building.IsAlive)
        {
            return false;
        }

        if (building.health >= building.maxHealth || repairAmount <= 0f)
        {
            return false;
        }

        if (requireHeld)
        {
            if (grabInteractable == null || !grabInteractable.isSelected)
            {
                return false;
            }
        }

        if (nextRepairTimes.TryGetValue(building, out float nextRepairTime))
        {
            if (Time.time < nextRepairTime)
            {
                return false;
            }
        }

        building.RestoreHealth(repairAmount);
        nextRepairTimes[building] = Time.time + repairCooldown;
        return true;
    }

    #endregion
}
