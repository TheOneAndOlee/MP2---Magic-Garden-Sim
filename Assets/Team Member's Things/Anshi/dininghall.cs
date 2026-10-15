
using UnityEngine;
using TMPro;

public class dininghall : MonoBehaviour
{
    public static float currentMoneyRate = 5f;

    public float moneyRate = 5f;
    public TMP_Text revenueText;

    private float totalRevenue = 0f;

    void Start()
    {
        moneyRate = currentMoneyRate;
        UpdateRevenueText();
    }

    void Update()
    {
        moneyRate = currentMoneyRate;

        float earned = moneyRate * Time.deltaTime;

        MoneyManager.Instance.AddMoney(earned);

        totalRevenue += earned;
        UpdateRevenueText();
    }

    void UpdateRevenueText()
    {
        if (revenueText != null)
        {
            revenueText.text = "Dining Hall Revenue: $" +
                               totalRevenue.ToString("F2");
        }
    }
}
