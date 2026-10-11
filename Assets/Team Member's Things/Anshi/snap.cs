using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class snap : MonoBehaviour
{
    public GridManager gridManager;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Vector2Int cell = gridManager.WorldToCell(transform.position);
        transform.position = gridManager.CellToWorld(cell);
        transform.rotation = Quaternion.identity; // (0,0,0)
    }
}