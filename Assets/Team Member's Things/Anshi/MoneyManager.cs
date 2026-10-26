using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public float money = 200;
    public TextMeshPro moneyText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
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
    private void UpdateUI()
    {
        moneyText.text ="Flowers Left: " +money;
    }
}