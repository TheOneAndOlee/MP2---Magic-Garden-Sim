using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class snap : MonoBehaviour
{
    public GridManager gridManager;

    private XRGrabInteractable grabInteractable;

    public void SetGridManager(GridManager manager)
    {
        gridManager = manager;
    }

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager has not been assigned!", this);
            return;
        }

        Vector2Int cell = gridManager.WorldToCell(transform.position);

        transform.position = gridManager.CellToWorld(cell);

        if (CompareTag("bookstore") || CompareTag("diningj"))
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        grabInteractable.enabled = false;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}