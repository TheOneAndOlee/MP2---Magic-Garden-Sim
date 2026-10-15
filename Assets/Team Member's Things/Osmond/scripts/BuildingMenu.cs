using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingMenu : MonoBehaviour
{
    #region Variables

    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private BuildingChoice choicePrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button convertButton;

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

        foreach (Building_Data buildingData in buildingManager.Inventory.AvailableBuildings)
        {
            if (buildingData == null)
            {
                continue;
            }

            BuildingChoice buildingChoice = Instantiate(choicePrefab, content);
            buildingChoice.Configure(buildingData, buildingManager);
        }

        if (convertButton != null)
        {
            convertButton.onClick.AddListener(ConvertResearch);
        }
    }

    private void Update()
    {
        UpdateStatusText();
    }

    #endregion

    #region Menu Methods

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

        string stats = "Output: " + inventory.ResearchOutput.ToString("0.0") + "/s";
        stats += " | Funds: +" + inventory.ResearchFundsOutput.ToString("0.0") + "/s";
        stats += " | Efficiency: " + inventory.ResearchEfficiency.ToString("0.0") + "%";
        stats += " | Morale: " + inventory.Morale.ToString("0.0");
        stats += " | Convert: " + inventory.ResearchPointThreshold.ToString("0.0") + " research";
        stats += " + " + inventory.ResearchConversionFunds.ToString("0.0") + " funds";

        statusText.text = buildingManager.Status + "\n" + resources + "\n" + stats;
    }

    public void ConvertResearch()
    {
        buildingManager.Inventory.TryConvertResearch();
    }

    #endregion
}
