using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

[RequireComponent(typeof(GridManager2), typeof(PlayerInventory))]
public class BuildingManager : MonoBehaviour
{
    #region Variables and their Getters & Setters

    [Header("Building System References")]
    [SerializeField] private GridManager2 grid;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform placementRayOrigin;
    [SerializeField] private Material previewMaterial;

    [Header("Building Placement")]
    [SerializeField] private LayerMask placementSurfaces = 1;

    [Tooltip("How far away the player can place a building")]
    [SerializeField, Min(1)]
    private float placementDistance = 15f;

    [Header("Starting Building")]
    [SerializeField] private Building_Data startingBuilding;

    [Tooltip("Minimum grid cell where the starting building can spawn")]
    [SerializeField] private Vector2Int startingSpawnMin = new Vector2Int(-4, 1);

    [Tooltip("Maximum grid cell where the starting building can spawn")]
    [SerializeField] private Vector2Int startingSpawnMax = new Vector2Int(3, 5);

    [Header("Pizza Spawning")]
    [SerializeField] private Building_Data pizzaBuilding;
    [SerializeField, Min(0.1f)] private float pizzaSpawnInterval = 30f;

    [Tooltip("Minimum grid cell where pizzas can spawn")]
    [SerializeField] private Vector2Int pizzaSpawnMin = new Vector2Int(-10, -10);

    [Tooltip("Maximum grid cell where pizzas can spawn")]
    [SerializeField] private Vector2Int pizzaSpawnMax = new Vector2Int(9, 9);

    private float pizzaSpawnTimer = 0f;

    private Building_Data selectedBuilding;
    private int rotationSteps;
    private bool canConfirmPlacement = false;

    private InputAction placeAction;
    private InputAction cancelAction;
    private InputAction rotateAction;

    private NearFarInteractor nearFarInteractor;
    private XRRayInteractor rayInteractor;

    private GameObject placementPreview;
    private Material activePreviewMaterial;

    public PlayerInventory Inventory
    {
        get => inventory;
    }

    public Building_Data SelectedBuilding
    {
        get => selectedBuilding;
    }

    public string Status { get; private set; } = "Choose a building, aim at the floor, then press the right trigger.";

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (grid == null)
        {
            grid = GetComponent<GridManager2>();
        }

        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }

        placeAction = new InputAction("Place building", InputActionType.Button, "<XRController>{RightHand}/triggerPressed");
        cancelAction = new InputAction("Cancel building", InputActionType.Button, "<XRController>{RightHand}/secondaryButton");
        rotateAction = new InputAction("Rotate building", InputActionType.Button, "<XRController>{RightHand}/primaryButton");
    }

    private void Start()
    {
        FindControllerReferences();
        RegisterExistingBuildings();
        SpawnStartingBuilding();
    }

    private void OnEnable()
    {
        placeAction.Enable();
        cancelAction.Enable();
        rotateAction.Enable();
    }

    private void OnDisable()
    {
        placeAction.Disable();
        cancelAction.Disable();
        rotateAction.Disable();

        CancelPlacement();
    }

    private void Update()
    {
        TickPizzaSpawning(Time.deltaTime);

        if (selectedBuilding == null)
        {
            return;
        }

        // Release the trigger after choosing a menu button before using it to build.
        if (!placeAction.IsPressed())
        {
            canConfirmPlacement = true;
        }

        if (cancelAction.WasPressedThisFrame())
        {
            CancelPlacement();
            return;
        }

        if (rotateAction.WasPressedThisFrame())
        {
            rotationSteps = (rotationSteps + 1) % 4;
        }

        if (!TryGetPlacementHit(out RaycastHit hit))
        {
            HidePreview();
            return;
        }

        Vector2Int cell = grid.WorldToCell(hit.point);
        bool canBuild = CanPlace(selectedBuilding, cell);
        ShowPreview(cell, canBuild);

        if (canConfirmPlacement && placeAction.WasPressedThisFrame())
        {
            TryPlace(hit.point, out Building placedBuilding);
        }
    }

    private void OnDestroy()
    {
        if (placeAction != null)
        {
            placeAction.Dispose();
        }

        if (cancelAction != null)
        {
            cancelAction.Dispose();
        }

        if (rotateAction != null)
        {
            rotateAction.Dispose();
        }

        if (placementPreview != null)
        {
            Destroy(placementPreview);
        }

        if (activePreviewMaterial != null)
        {
            Destroy(activePreviewMaterial);
        }
    }

    #endregion

    #region Building Placement Methods

    public void SelectBuilding(Building_Data data)
    {
        if (!inventory.IsAvailable(data) || data.buildingPrefab == null)
        {
            return;
        }

        selectedBuilding = data;
        canConfirmPlacement = false;
        Status = data.buildingName + ": aim at the floor; trigger places, A rotates, B cancels.";
    }

    public void CancelPlacement()
    {
        selectedBuilding = null;
        HidePreview();
        Status = "Choose a building, aim at the floor, then press the right trigger.";
    }

    private bool CanPlace(Building_Data data, Vector2Int cell)
    {
        if (data == null || data.buildingPrefab == null || !inventory.IsAvailable(data))
        {
            return false;
        }

        if (data.buildingPrefab.GetComponent<Building>() == null)
        {
            return false;
        }

        if (!inventory.CanAfford(data))
        {
            return false;
        }

        return IsCellClear(cell);
    }

    private bool IsCellClear(Vector2Int cell)
    {
        if (!grid.IsFree(cell))
        {
            return false;
        }

        // Check above the floor so walls, furniture and tools can block this cell.
        Vector3 cellCentre = grid.CellToWorld(cell) + Vector3.up * 0.5f;
        Vector3 halfSize = new Vector3(0.48f, 0.45f, 0.48f);
        Collider[] obstacles = Physics.OverlapBox(cellCentre, halfSize, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);

        return obstacles.Length == 0;
    }

    public bool TryPlace(Vector3 worldPosition, out Building building)
    {
        building = null;
        Vector2Int cell = grid.WorldToCell(worldPosition);

        if (!CanPlace(selectedBuilding, cell))
        {
            Status = "Cannot build here: check the cell, obstacles, and resources.";
            return false;
        }

        if (!inventory.TrySpend(selectedBuilding))
        {
            return false;
        }

        GameObject placedObject = null;

        try
        {
            Vector3 buildingPosition = grid.CellToWorld(cell);
            Quaternion buildingRotation = Quaternion.Euler(0f, rotationSteps * 90f, 0f);
            placedObject = Instantiate(selectedBuilding.buildingPrefab, buildingPosition, buildingRotation);

            if (!grid.TryRegister(cell, placedObject))
            {
                Destroy(placedObject);
                inventory.Refund(selectedBuilding);
                return false;
            }

            building = placedObject.GetComponent<Building>();
            building.Initialize(selectedBuilding, inventory, grid, cell);
            Status = "Placed " + selectedBuilding.buildingName + ".";

            return true;
        }
        catch (System.Exception exception)
        {
            // Return the resources if something went wrong after they were spent.
            if (placedObject != null)
            {
                grid.Unregister(cell, placedObject);
                Destroy(placedObject);
            }

            inventory.Refund(selectedBuilding);
            Debug.LogException(exception, this);
            return false;
        }
    }

    #endregion

    #region Random Building Spawns

    private void SpawnStartingBuilding()
    {
        if (startingBuilding == null)
        {
            return;
        }

        if (!TrySpawnRandomBuilding(startingBuilding, startingSpawnMin, startingSpawnMax, out Building building))
        {
            Debug.LogWarning("The starting building needs a valid prefab and a free floor cell in its spawn area.", this);
        }
    }

    public void TickPizzaSpawning(float seconds)
    {
        if (pizzaBuilding == null)
        {
            return;
        }

        pizzaSpawnTimer += Mathf.Max(0f, seconds);
        float interval = Mathf.Max(0.1f, pizzaSpawnInterval);

        while (pizzaSpawnTimer >= interval)
        {
            pizzaSpawnTimer -= interval;

            // A full spawn area skips this pizza and tries again at the next interval.
            TrySpawnRandomBuilding(pizzaBuilding, pizzaSpawnMin, pizzaSpawnMax, out Building building);
        }
    }

    public bool TrySpawnRandomBuilding(Building_Data data, Vector2Int spawnMin, Vector2Int spawnMax, out Building building)
    {
        building = null;

        if (data == null || data.buildingPrefab == null || data.buildingPrefab.GetComponent<Building>() == null)
        {
            return false;
        }

        Physics.SyncTransforms();

        List<Vector2Int> availableCells = new List<Vector2Int>();
        Vector2Int minCell = Vector2Int.Min(spawnMin, spawnMax);
        Vector2Int maxCell = Vector2Int.Max(spawnMin, spawnMax);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int z = minCell.y; z <= maxCell.y; z++)
            {
                Vector2Int cell = new Vector2Int(x, z);

                if (IsCellClear(cell) && HasFloorAtCell(cell))
                {
                    availableCells.Add(cell);
                }
            }
        }

        if (availableCells.Count == 0)
        {
            return false;
        }

        int randomIndex = Random.Range(0, availableCells.Count);
        Vector2Int spawnCell = availableCells[randomIndex];
        Vector3 spawnPosition = grid.CellToWorld(spawnCell);
        GameObject spawnedObject = Instantiate(data.buildingPrefab, spawnPosition, Quaternion.identity);

        if (!grid.TryRegister(spawnCell, spawnedObject))
        {
            Destroy(spawnedObject);
            return false;
        }

        // World spawns are free and contribute their normal stat bonuses.
        building = spawnedObject.GetComponent<Building>();
        building.Initialize(data, inventory, grid, spawnCell);
        return true;
    }

    private bool HasFloorAtCell(Vector2Int cell)
    {
        Vector3 rayOrigin = grid.CellToWorld(cell) + Vector3.up;
        bool hitSomething = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
            1.1f, placementSurfaces, QueryTriggerInteraction.Ignore);

        if (!hitSomething || hit.normal.y < 0.95f)
        {
            return false;
        }

        if (Mathf.Abs(hit.point.y - grid.transform.position.y) > 0.05f)
        {
            return false;
        }

        return hit.collider.GetComponentInParent<Building>() == null;
    }

    #endregion

    #region Controller and Raycast Helpers

    private void FindControllerReferences()
    {
        // Controllers may start inactive until the XR rig detects them.
        NearFarInteractor[] nearFarInteractors = FindObjectsByType<NearFarInteractor>(FindObjectsInactive.Include);

        foreach (NearFarInteractor interactor in nearFarInteractors)
        {
            if (IsRightHand(interactor.transform))
            {
                nearFarInteractor = interactor;
                break;
            }
        }

        XRRayInteractor[] rayInteractors = FindObjectsByType<XRRayInteractor>(FindObjectsInactive.Include);

        foreach (XRRayInteractor interactor in rayInteractors)
        {
            if (IsRightHand(interactor.transform))
            {
                rayInteractor = interactor;
                break;
            }
        }

        if (placementRayOrigin == null)
        {
            if (nearFarInteractor != null)
            {
                placementRayOrigin = nearFarInteractor.transform;
            }
            else if (rayInteractor != null)
            {
                placementRayOrigin = rayInteractor.transform;
            }
        }
    }

    private static bool IsRightHand(Transform target)
    {
        Transform currentParent = target;

        while (currentParent != null)
        {
            if (currentParent.name.Contains("Right"))
            {
                return true;
            }

            currentParent = currentParent.parent;
        }

        return false;
    }

    private bool TryGetPlacementHit(out RaycastHit hit)
    {
        hit = new RaycastHit();

        bool pointingAtUI = nearFarInteractor != null && nearFarInteractor.TryGetCurrentUIRaycastResult(out _);
        pointingAtUI = pointingAtUI || (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out _));

        bool holdingObject = nearFarInteractor != null && nearFarInteractor.hasSelection;
        holdingObject = holdingObject || (rayInteractor != null && rayInteractor.hasSelection);

        if (placementRayOrigin == null || pointingAtUI || holdingObject)
        {
            return false;
        }

        bool hitSomething = Physics.Raycast(placementRayOrigin.position, placementRayOrigin.forward,
            out hit, placementDistance, placementSurfaces, QueryTriggerInteraction.Ignore);

        if (!hitSomething)
        {
            return false;
        }

        // Only build on a flat floor at the grid's height, not on another building.
        if (hit.normal.y < 0.95f || Mathf.Abs(hit.point.y - grid.transform.position.y) > 0.05f)
        {
            return false;
        }

        if (hit.collider.GetComponentInParent<Building>() != null)
        {
            return false;
        }

        return true;
    }

    private void RegisterExistingBuildings()
    {
        Building[] existingBuildings = FindObjectsByType<Building>();

        foreach (Building building in existingBuildings)
        {
            Vector2Int cell = grid.WorldToCell(building.transform.position);

            if (grid.IsFree(cell))
            {
                building.Initialize(building.Data, inventory, grid, cell);
            }
        }
    }

    #endregion

    #region Placement Preview

    private void ShowPreview(Vector2Int cell, bool canBuild)
    {
        if (placementPreview == null)
        {
            CreatePreview();
        }

        placementPreview.SetActive(true);
        placementPreview.transform.position = grid.CellToWorld(cell) + Vector3.up * 0.015f;

        if (canBuild)
        {
            activePreviewMaterial.color = new Color(0.1f, 0.8f, 0.3f);
        }
        else
        {
            activePreviewMaterial.color = new Color(0.9f, 0.15f, 0.1f);
        }
    }

    private void CreatePreview()
    {
        placementPreview = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placementPreview.name = "Building cell preview";
        placementPreview.layer = 2;
        placementPreview.GetComponent<Collider>().enabled = false;
        placementPreview.transform.localScale = new Vector3(0.96f, 0.015f, 0.96f);

        if (previewMaterial != null)
        {
            activePreviewMaterial = new Material(previewMaterial);
        }
        else
        {
            Shader previewShader = Shader.Find("Universal Render Pipeline/Unlit");

            if (previewShader == null)
            {
                previewShader = Shader.Find("Unlit/Color");
            }

            activePreviewMaterial = new Material(previewShader);
        }

        placementPreview.GetComponent<Renderer>().sharedMaterial = activePreviewMaterial;
    }

    private void HidePreview()
    {
        if (placementPreview != null)
        {
            placementPreview.SetActive(false);
        }
    }

    #endregion
}
