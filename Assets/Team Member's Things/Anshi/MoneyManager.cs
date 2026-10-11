using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    public int money = 200;
    public TextMeshPro moneyText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount)
            return false;

        money -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        moneyText.text = "$" + money;
    }
}