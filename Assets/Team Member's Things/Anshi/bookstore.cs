
using UnityEngine;
using TMPro;

public class bookstore : MonoBehaviour
{
    public float moneyRate = 1f;
    public TMP_Text revenueText;

    private float totalRevenue = 0f;

    void Start()
    {
        UpdateRevenueText();
    }

    void Update()
    {
        float earned = moneyRate * Time.deltaTime;

        MoneyManager.Instance.AddMoney(earned);

        totalRevenue += earned;
        UpdateRevenueText();
    }

    void UpdateRevenueText()
    {
        if (revenueText != null)
        {
            revenueText.text = "Bookstore Revenue: $" +
                               totalRevenue.ToString("F2");
        }
    }
}
