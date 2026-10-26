
using UnityEngine;

public class upgrade : MonoBehaviour
{
    public float rateIncrease = 5f;
    public int upgradeCost = 100;
    public GameObject dhPrefab;
    private GameObject dhInstance;

    public reveal unlockCardPrefab;

    private reveal unlockCardInstance;
    public void Upgrade()
    {
        GameObject bookstore = GameObject.FindGameObjectWithTag("diningj");

        if (
            bookstore != null )
        {
            if (!MoneyManager.Instance.SpendMoney(upgradeCost))
        {
            return;
        }
        unlockCardInstance.ShowCard("School");

        dininghall.currentMoneyRate += rateIncrease;
        } else {
            
                dhInstance = Instantiate(
                    dhPrefab,
                    transform.position + new Vector3(-2, 0, 0),
                    Quaternion.Euler(0, 0, 90)
                );

                Destroy(dhInstance, 5f);
            
                }
        

    }
}
