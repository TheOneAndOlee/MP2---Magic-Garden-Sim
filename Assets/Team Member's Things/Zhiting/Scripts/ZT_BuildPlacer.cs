using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using System;

[System.Serializable]
public class ZT_BuildOption
{
    public GameObject prefab;
    public ZT_ResourceType costType;
    public float cost;
    public ZT_Stage unlockStage = ZT_Stage.BuildTutorial;
    public bool needsGraduate = false;  
}
public class ZT_BuildPlacer : MonoBehaviour
{
    // build
    public GridManager grid;
    public Transform rayOrigin;
    public float maxDistance = 10f;

    public GameObject preview;
    private Renderer previewRenderer;

    public Material validMaterial;
    public Material invalidMaterial;

    //switch building
    public List<ZT_BuildOption> options;
    private int currentIndex = 0;
    public InputAction switchAction = new InputAction(binding: "<XRController>{RightHand}/secondaryButton");

    // open/close buildMode
    private bool buildMode = false;
    public InputAction toggleAction = new InputAction(binding: "<XRController>{RightHand}/primaryButton");

    // demolish
    public InputAction demolishAction = new InputAction(binding: "<XRController>{LeftHand}/primaryButton");

    //place
    public InputAction placeAction = new InputAction(binding: "<XRController>{RightHand}/triggerPressed");

    //displaly
    public TMP_Text buildLabel;

    //event
    public static event Action<ZT_BuildPlacer> EnterBuild;
    public static event Action<ZT_BuildPlacer> PlaceBed;
    public static event Action<ZT_BuildPlacer> Demolish;

    private bool broadcastEnterBuild = false;
    private bool broadcastPlaceBed = false;
    private bool broadcastDemolish = false;

    private Dictionary<GameObject, ZT_BuildOption> builtOptions = new Dictionary<GameObject, ZT_BuildOption>();

    // get graduate
    private GradManager gradManager;
    [SerializeField] private int graduatesPerMachine = 1;

    void Start()
    {
        previewRenderer = preview.GetComponent<Renderer>();
        preview.SetActive(false);
    }
    
    void OnEnable()
    {
        toggleAction.Enable();
        switchAction.Enable();
        demolishAction.Enable();
        placeAction.Enable();
    }

    void OnDisable()
    {
        toggleAction.Disable();
        switchAction.Disable();
        demolishAction.Disable();
        placeAction.Disable();
    }


    private int GetTotalGraduates()
    {
        if (gradManager == null)
        {
            gradManager = FindAnyObjectByType<GradManager>();
        }
        if (gradManager == null) return 0;
        return gradManager.totalGraduated;
    }


    private int GetAllowedMachines()
    {
        return GetTotalGraduates() / Mathf.Max(1, graduatesPerMachine);
    }

    private int CountStaffedBuildings()
    {
        int count = 0;
        foreach (var pair in builtOptions)
        {
            if (pair.Key != null && pair.Value.needsGraduate) count++;
        }
        return count;
    }

  


    void Update()
    {

        //open/close BuildMode
        if (toggleAction.WasPressedThisFrame())
        {
            if (ZT_ProgressManager.Current != null && !ZT_ProgressManager.Current.HasReached(ZT_Stage.BuildTutorial))
            {
                ZT_Message.Show("Building unlocks after you harvest your first flower.");
            }
            else
            {
                buildMode = !buildMode;
                if (!broadcastEnterBuild)
                {
                    broadcastEnterBuild = true;
                    EnterBuild?.Invoke(this);
                }
            }
        }

        if (!buildMode)
        {
            preview.SetActive(false);
            buildLabel.text = "";
            return;
        }

        ZT_BuildOption opt = options[currentIndex];
        string label = "Building: " + opt.prefab.name;

        if (opt.cost > 0)
        { 
            label += "\nCost: " + opt.cost + " " + opt.costType;
        }

        if (opt.needsGraduate)
        {
            label += "\nRain Machines: " + CountStaffedBuildings() + " / " + GetAllowedMachines()
                + " (" + GetTotalGraduates() + " graduates, every " + graduatesPerMachine + " allow 1)";
        }

        buildLabel.text = label;

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
            bool unlocked = ZT_ProgressManager.Current == null || ZT_ProgressManager.Current.HasReached(options[currentIndex].unlockStage);
            bool enoughGraduates = !options[currentIndex].needsGraduate || CountStaffedBuildings() < GetAllowedMachines();
            bool enoughResources = enoughGraduates
                && ZT_Resources.Instance.GetResources(options[currentIndex].costType) >= options[currentIndex].cost;
            bool canBuild = unlocked
                && grid.IsFree(cell)
                && enoughResources
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
            if (placeAction.WasPressedThisFrame())
            {
                if (!unlocked)
                {
                    ZT_Message.Show("The Rain Machine unlocks after you harvest 2 flowers.");
                }
                else if (!enoughGraduates)
                {
                    ZT_Message.Show("You can run " + GetAllowedMachines() + " Rain Machine(s) with " + GetTotalGraduates() + " graduates. Every " + graduatesPerMachine + " graduates from the School allow one more.");
                }
                else if (!enoughResources)
                {
                    ZT_Message.Show("Not enough " + options[currentIndex].costType + " to build this.");

                }
                else if (canBuild)
                {
                    ZT_Resources.Instance.TrySpend(options[currentIndex].costType, options[currentIndex].cost);
                    GameObject building = Instantiate(options[currentIndex].prefab, position, Quaternion.identity);
                    grid.Register(cell, building);
                    builtOptions[building] = options[currentIndex];
                    if (!broadcastPlaceBed)
                    {
                        broadcastPlaceBed = true;
                        PlaceBed?.Invoke(this);
                    }
                    ZT_RainMachine machine = building.GetComponent<ZT_RainMachine>();
                    if (machine != null)
                    {
                        machine.SetGrid(grid, cell);
                    }
                }

            }

            if (demolishAction.WasPressedThisFrame() && !grid.IsFree(cell))
            {
                GameObject target = grid.GetAt(cell);

                ZT_Bed bed = target.GetComponentInChildren<ZT_Bed>();
                if (bed != null && bed.HasFlower)
                {
                    ZT_Message.Show("Harvest the flower before removing this bed.");
                    return;
                }

                if (builtOptions.TryGetValue(target, out ZT_BuildOption option))
                {


                    ZT_Resources.Instance.AddResources(option.costType, (int)option.cost);
                    builtOptions.Remove(target);
                }

                grid.Unregister(cell);
                Destroy(target);

                if (!broadcastDemolish)
                {
                    broadcastDemolish = true;
                    Demolish?.Invoke(this);
                }
                if (ZT_ProgressManager.Current != null)
                {
                    ZT_ProgressManager.Current.ReportDemolish();
                }
            }
        }
        else
        {
            preview.SetActive(false);
        }
    }
}
