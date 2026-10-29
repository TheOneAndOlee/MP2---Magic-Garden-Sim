using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingMenu : MonoBehaviour
{
    #region Variables

    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private BuildingChoice choicePrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button convertButton;
    [SerializeField] private ProgressionMenu progressionMenu;
    private HashSet<Building_Data> displayedBuildings = new HashSet<Building_Data>();

    #endregion

    #region Unity Methods

    private void Start()
    {
        GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();

        if (layout == null)
        {
            layout = content.gameObject.AddComponent<GridLayoutGroup>();
        }

        layout.cellSize = new Vector2(370f, 150f);
        layout.spacing = new Vector2(15f, 15f);
        layout.padding = new RectOffset(30, 30, 30, 30);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 3;

        RefreshBuildingChoices();
        buildingManager.Inventory.Progression.Changed += RefreshBuildingChoices;

        if (progressionMenu != null)
        {
            progressionMenu.Initialize(buildingManager);
        }

        if (convertButton != null)
        {
            convertButton.onClick.AddListener(ConvertResearch);
        }
    }

    private void OnDestroy()
    {
        if (buildingManager != null && buildingManager.Inventory != null && buildingManager.Inventory.Progression != null)
        {
            buildingManager.Inventory.Progression.Changed -= RefreshBuildingChoices;
        }
    }

    private void Update()
    {
        UpdateStatusText();
    }

    #endregion

    #region Menu Methods

    private void RefreshBuildingChoices()
    {
        foreach (Building_Data data in buildingManager.Inventory.AvailableBuildings)
        {
            if (data == null || displayedBuildings.Contains(data) || !buildingManager.Inventory.IsAvailable(data))
            {
                continue;
            }

            BuildingChoice choice = Instantiate(choicePrefab, content);
            choice.Configure(data, buildingManager);
            displayedBuildings.Add(data);
        }
    }

    private void UpdateStatusText()
    {
        if (statusText == null)
        {
            return;
        }

        PlayerInventory inventory = buildingManager.Inventory;

        string funds = inventory.GetAmount(SimResource.ResourceType.ResearchFunds).ToString("0");
        string progress = inventory.GetAmount(SimResource.ResourceType.ResearchProgressPoints).ToString("0");
        string maximumProgress = inventory.MaximumResearchProgress.ToString("0");
        string points = inventory.GetAmount(SimResource.ResourceType.ResearchPoints).ToString("0");
        string maximumPoints = inventory.MaximumResearchPoints.ToString("0");

        string resources = "Funds: " + funds + " | Research: " + progress + "/" + maximumProgress;
        resources += " | Points: " + points + "/" + maximumPoints;

        string stats = "Output: " + inventory.ResearchPerSecond.ToString("0.0") + "/s";
        stats += " | Funds: +" + inventory.ResearchFundsOutput.ToString("0.0") + "/s";
        stats += " | Efficiency: " + inventory.ResearchEfficiency.ToString("100.0") + "%";
        stats += " | Morale: " + inventory.Morale.ToString("0.0");
        stats += " | Convert: " + inventory.ResearchPointThreshold.ToString("0.0") + " research";
        stats += " + " + inventory.ResearchConversionFunds.ToString("0.0") + " funds";

        statusText.text = buildingManager.Status + "\n" + resources + "\n" + stats;
    }

    public void ConvertResearch()
    {
        if (!buildingManager.Inventory.TryConvertResearch())
        {
            buildingManager.Inventory.Progression.Notify("Cannot convert yet: check your RPP, funds, and Research Point capacity.", true);
        }
        else
        {
            buildingManager.Inventory.Progression.CheckAchievements();
        }
    }

    #endregion
}
