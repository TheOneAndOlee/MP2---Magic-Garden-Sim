using UnityEngine;
using UnityEngine.UI;

public class StoreItemManager : MonoBehaviour
{
    [Header("Teammate Integration")]
    public ResearchProgression researchManager;
    [Tooltip("The exact string ID from the ResearchTreeData needed to unlock the base item.")]
    public string requiredResearchID = "";
    [Tooltip("The string IDs needed to unlock each upgrade level (e.g., tower-1).")]
    public string[] upgradeResearchIDs = new string[3];

    [Header("Hierarchy Models")]
    public GameObject lockedModel;
    public GameObject unlockedModel;

    [Header("Weapon Link")]
    public LaserPointerTargeter weaponScript;

    [Header("Upgrade UI Elements")]
    public Button upgradeButton;
    public MeshRenderer[] statusSpheres;

    [Header("Sphere Colors")]
    public Material redMaterial;
    public Material greenMaterial;

    [Header("Costs")]
    public int purchaseCost = 1000;
    public int[] upgradeCosts = new int[3];

    [HideInInspector] public bool isPlacedOnGrid = false;
    public int currentUpgradeLevel = 0;
    private bool isUpgraded = false;
    private bool itemPermanentlyUnlocked = false;

    void Start()
    {
        UpdateUnlockState();

        foreach (MeshRenderer sphere in statusSpheres)
        {
            if (sphere != null) sphere.material = redMaterial;
        }
        if (upgradeButton != null) upgradeButton.interactable = false;
    }

    void Update()
    {
        if (isUpgraded && currentUpgradeLevel >= statusSpheres.Length) return;

        if (isPlacedOnGrid)
        {
            itemPermanentlyUnlocked = true;
            if (lockedModel != null && lockedModel.activeSelf) lockedModel.SetActive(false);
        }
        else if (!itemPermanentlyUnlocked)
        {
            UpdateUnlockState();
        }

        if (currentUpgradeLevel >= statusSpheres.Length)
        {
            if (upgradeButton != null) upgradeButton.interactable = false;
            isUpgraded = true;
        }
        else
        {
            bool canAffordNextUpgrade = GameManager.Instance.currentCurrency >= upgradeCosts[currentUpgradeLevel];

            bool isUpgradeResearched = true;
            if (researchManager != null && currentUpgradeLevel < upgradeResearchIDs.Length)
            {
                string requiredUpgradeID = upgradeResearchIDs[currentUpgradeLevel];
                if (!string.IsNullOrEmpty(requiredUpgradeID))
                {
                    isUpgradeResearched = researchManager.IsResearched(requiredUpgradeID);
                }
            }

            if (upgradeButton != null)
            {
                upgradeButton.interactable = isPlacedOnGrid && canAffordNextUpgrade && isUpgradeResearched;
            }
        }
    }

    private void UpdateUnlockState()
    {
        bool researchCompleted = true;
        if (researchManager != null && !string.IsNullOrEmpty(requiredResearchID))
        {
            researchCompleted = researchManager.IsResearched(requiredResearchID);
        }

        bool canAffordPurchase = GameManager.Instance.currentCurrency >= purchaseCost;
        bool isUnlocked = researchCompleted && canAffordPurchase;

        if (lockedModel != null && unlockedModel != null)
        {
            if (lockedModel.activeSelf == isUnlocked || unlockedModel.activeSelf != isUnlocked)
            {
                lockedModel.SetActive(!isUnlocked);
                unlockedModel.SetActive(isUnlocked);
            }
        }
    }

    public void NotifyTowerPlaced()
    {
        isPlacedOnGrid = true;
        if (lockedModel != null) lockedModel.SetActive(false);
    }

    public void PurchaseUpgrade()
    {
        if (isPlacedOnGrid && currentUpgradeLevel < statusSpheres.Length)
        {
            int costOfNextUpgrade = upgradeCosts[currentUpgradeLevel];

            bool isUpgradeResearched = true;
            if (researchManager != null && currentUpgradeLevel < upgradeResearchIDs.Length)
            {
                string requiredUpgradeID = upgradeResearchIDs[currentUpgradeLevel];
                if (!string.IsNullOrEmpty(requiredUpgradeID))
                {
                    isUpgradeResearched = researchManager.IsResearched(requiredUpgradeID);
                }
            }

            if (isUpgradeResearched && GameManager.Instance.SpendCurrency(costOfNextUpgrade))
            {
                if (statusSpheres[currentUpgradeLevel] != null)
                {
                    statusSpheres[currentUpgradeLevel].material = greenMaterial;
                }

                currentUpgradeLevel++;

                if (weaponScript != null)
                {
                    weaponScript.weaponUpgradeLevel = currentUpgradeLevel;
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.IncreaseAlienSpawns();
                }
            }
        }
    }
}