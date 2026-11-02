using UnityEngine;
using UnityEngine.UI;

public class StoreItemManager : MonoBehaviour
{
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

    [Header("Costs (Alien USD)")]
    public int purchaseCost = 1000;
    public int[] upgradeCosts = new int[3];

    [HideInInspector] public bool isPlacedOnGrid = false;
    public int currentUpgradeLevel = 0;

    private bool isResearchedInTree = false;
    private bool[] isUpgradeResearchedInTree = new bool[3];

    void Start()
    {
        foreach (MeshRenderer sphere in statusSpheres)
        {
            if (sphere != null) sphere.material = redMaterial;
        }
        if (upgradeButton != null) upgradeButton.interactable = false;

        UpdateModels();
    }

    void Update()
    {
        if (!isPlacedOnGrid)
        {
            UpdateModels();
        }

        UpdateUpgradeButton();
    }

    public void UnlockFromResearchTree()
    {
        isResearchedInTree = true;
        UpdateModels();
    }

    public void UnlockUpgradeFromResearchTree(int upgradeIndex)
    {
        if (upgradeIndex >= 0 && upgradeIndex < isUpgradeResearchedInTree.Length)
        {
            isUpgradeResearchedInTree[upgradeIndex] = true;
        }
    }

    private void UpdateModels()
    {
        bool hasEnoughUSD = GameManager.Instance.currentCurrency >= purchaseCost;
        bool isFullyUnlocked = isResearchedInTree && hasEnoughUSD;

        if (lockedModel != null) lockedModel.SetActive(!isFullyUnlocked);
        if (unlockedModel != null) unlockedModel.SetActive(isFullyUnlocked);
    }

    public void NotifyTowerPlaced()
    {
        isPlacedOnGrid = true;
        if (lockedModel != null) lockedModel.SetActive(false);
        if (unlockedModel != null) unlockedModel.SetActive(true);
    }

    private void UpdateUpgradeButton()
    {
        if (currentUpgradeLevel >= statusSpheres.Length || upgradeButton == null)
        {
            if (upgradeButton != null) upgradeButton.interactable = false;
            return;
        }

        bool hasEnoughUSD = GameManager.Instance.currentCurrency >= upgradeCosts[currentUpgradeLevel];
        bool isTreeUnlocked = isUpgradeResearchedInTree[currentUpgradeLevel];

        upgradeButton.interactable = isPlacedOnGrid && hasEnoughUSD && isTreeUnlocked;
    }

    public void PurchaseUpgrade()
    {
        if (!isPlacedOnGrid || currentUpgradeLevel >= statusSpheres.Length) return;

        bool isTreeUnlocked = isUpgradeResearchedInTree[currentUpgradeLevel];

        if (isTreeUnlocked && GameManager.Instance.SpendCurrency(upgradeCosts[currentUpgradeLevel]))
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