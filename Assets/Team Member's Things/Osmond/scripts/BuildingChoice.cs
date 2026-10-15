using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BuildingChoice : MonoBehaviour
{
    #region Variables

    [Header("Menu References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image buildingImage;
    [SerializeField] private Building_Data building_Data;

    private BuildingManager buildingManager;
    private Button choiceButton;
    private float nextRefreshTime;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        choiceButton = GetComponent<Button>();
        choiceButton.onClick.AddListener(SelectBuilding);
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime)
        {
            return;
        }

        nextRefreshTime = Time.unscaledTime + 0.1f;
        RefreshAvailability();
    }

    private void OnDestroy()
    {
        if (choiceButton != null)
        {
            choiceButton.onClick.RemoveListener(SelectBuilding);
        }
    }

    #endregion

    #region Menu Methods

    public void Configure(Building_Data data, BuildingManager manager)
    {
        building_Data = data;
        buildingManager = manager;

        if (nameText != null)
        {
            nameText.text = data.buildingName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = data.description;
        }

        string costs = "";

        if (data.buildingCosts != null)
        {
            foreach (Building_Data.BuildingCost buildingCost in data.buildingCosts)
            {
                if (costs.Length > 0)
                {
                    costs += " | ";
                }

                costs += buildingCost.cost.ToString("0") + " " + buildingCost.resource;
            }
        }

        if (costText != null)
        {
            if (costs.Length == 0)
            {
                costText.text = "Free";
            }
            else
            {
                costText.text = costs;
            }
        }

        if (buildingImage != null)
        {
            if (data.menuImage != null)
            {
                buildingImage.sprite = data.menuImage.sprite;
            }
            else
            {
                buildingImage.sprite = null;
            }

            buildingImage.enabled = buildingImage.sprite != null;
        }

        RefreshAvailability();
    }

    private void RefreshAvailability()
    {
        if (choiceButton == null)
        {
            return;
        }

        if (buildingManager == null || building_Data == null)
        {
            choiceButton.interactable = false;
            return;
        }

        choiceButton.interactable = buildingManager.Inventory.IsAvailable(building_Data)
            && buildingManager.Inventory.CanAfford(building_Data);
    }

    public void SelectBuilding()
    {
        if (buildingManager != null)
        {
            buildingManager.SelectBuilding(building_Data);
        }
    }

    #endregion
}
