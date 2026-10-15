
using UnityEngine;

public class upgrade : MonoBehaviour
{
    public float rateIncrease = 5f;
    public int upgradeCost = 100;

    public void Upgrade()
    {
        if (!MoneyManager.Instance.SpendMoney(upgradeCost))
        {
            return;
        }

        dininghall.currentMoneyRate += rateIncrease;

    }
}
