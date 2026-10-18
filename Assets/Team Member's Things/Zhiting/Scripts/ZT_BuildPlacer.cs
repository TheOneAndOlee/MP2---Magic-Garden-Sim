using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ZT_BuildOption
{
    public GameObject prefab;
    public ZT_ResourceType costType;
    public float cost;
}
public class ZT_BuildPlacer : MonoBehaviour
{
    // build
    public GridManager grid;
    public Transform rayOrigin;
    public float maxDistance = 10f;

    public GameObject preview;
    private Renderer previewRenderer;

    public InputActionReference placeAction;
    public Material validMaterial;
    public Material invalidMaterial;

    //switch building
    public List<ZT_BuildOption> options;
    private int currentIndex = 0;
    public InputAction switchAction = new InputAction(binding: "<XRController>{RightHand}/secondaryButton");

    // open/close buildMode
    private bool buildMode = false;
    public InputAction toggleAction = new InputAction(binding: "<XRController>{RightHand}/primaryButton");

    //displaly
    public TMP_Text buildLabel;

    void Start()
    {
        previewRenderer = preview.GetComponent<Renderer>();
        preview.SetActive(false);
    }
    
    void OnEnable()
    {
        toggleAction.Enable();
        switchAction.Enable();
    }

    void OnDisable()
    {
        toggleAction.Disable();
        switchAction.Disable();
    }
    void Update()
    {
        //open/close BuildMode
        if (toggleAction.WasPressedThisFrame())
        {
            buildMode = !buildMode;
        }

        if (!buildMode)
        {
            preview.SetActive(false);
            buildLabel.text = "";
            return;
        }
        buildLabel.text = "Building: " + options[currentIndex].prefab.name;

        //switch building
        if (switchAction.WasPressedThisFrame())
        {
            currentIndex = (currentIndex + 1) % options.Count;
            Debug.Log(options[currentIndex].prefab.name);
        }
        //build
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            //calculate position of ray
            Vector2Int cell = grid.WorldToCell(hit.point);
            Vector3 position = grid.CellToWorld(cell);
            preview.SetActive(true);
            preview.transform.position = position;

            //check status of cell
            bool canBuild = grid.IsFree(cell) 
                && ZT_Resources.Instance.GetResources(options[currentIndex].costType) >= options[currentIndex].cost
                && hit.collider.GetComponent<ZT_BuildArea>() != null;

            //show preview
            if (canBuild)
            {
                previewRenderer.material = validMaterial;
            }
            else
            {
                previewRenderer.material = invalidMaterial;
            }

            //build

            if (placeAction.action.WasPressedThisFrame() && canBuild)
            {
                ZT_Resources.Instance.TrySpend(options[currentIndex].costType, options[currentIndex].cost);
                GameObject building = Instantiate(options[currentIndex].prefab, position, Quaternion.identity);
                grid.Register(cell, building);

            }
        }
        else
        {
            preview.SetActive(false);
        }
    }
}
