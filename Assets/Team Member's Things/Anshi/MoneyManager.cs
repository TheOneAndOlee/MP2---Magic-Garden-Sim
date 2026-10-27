using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public float money = 200;
    public TextMeshPro moneyText;
    public TextMeshPro flowerText;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }
    void Update()
    {
        flowerText.text = "Flowers Left: " + ZT_Resources.Instance.GetResources(ZT_ResourceType.Flower);

    }
    public bool SpendMoney(float amount)
    {
        if (money < amount)
            return false;

        money -= amount;
        UpdateUI();
        return true;
    }
    public bool AddMoney(float amount)
    {
        
        money += amount;
        UpdateUI();
        return true;
    }
    public bool SpendFlower(float amount)
    {
        if (!ZT_Resources.Instance.TrySpend(ZT_ResourceType.Flower,1))
            return false;

        UpdateUI();
        return true;
    }
    private void UpdateUI()
    {
        moneyText.text ="Money Left: " +money;
        flowerText.text = "Flowers Left: " + ZT_Resources.Instance.GetResources(ZT_ResourceType.Flower);
    }
}