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

    [Header("Costs")]
    public int purchaseCost = 1000;
    public int[] upgradeCosts = new int[3]; 

    [HideInInspector] public bool isPlacedOnGrid = false;
    public int currentUpgradeLevel = 0; 
    private bool isUpgraded = false;
    private bool itemPermanentlyUnlocked = false; 

    void Start()
    {
        bool canAfford = GameManager.Instance.currentCurrency >= purchaseCost;
        if (lockedModel != null) lockedModel.SetActive(!canAfford);
        if (unlockedModel != null) unlockedModel.SetActive(canAfford);

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
            bool canAffordPurchase = GameManager.Instance.currentCurrency >= purchaseCost;
            
            if (lockedModel != null && unlockedModel != null)
            {
                if (lockedModel.activeSelf == canAffordPurchase || unlockedModel.activeSelf != canAffordPurchase)
                {
                    lockedModel.SetActive(!canAffordPurchase);
                    unlockedModel.SetActive(canAffordPurchase);
                }
            }
        }

        if (currentUpgradeLevel >= statusSpheres.Length)
        {
            if (upgradeButton != null) upgradeButton.interactable = false;
            isUpgraded = true;
        }
        else
        {
            bool canAffordNextUpgrade = GameManager.Instance.currentCurrency >= upgradeCosts[currentUpgradeLevel];
            if (upgradeButton != null) upgradeButton.interactable = isPlacedOnGrid && canAffordNextUpgrade;
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
            
            if (GameManager.Instance.SpendCurrency(costOfNextUpgrade))
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