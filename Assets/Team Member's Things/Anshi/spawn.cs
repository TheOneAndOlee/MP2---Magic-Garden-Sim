using UnityEngine;

public class spawn : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform rightHand;
    public int cost = 100;
    public void Spawn()
    {
        if (!MoneyManager.Instance.SpendMoney(cost))
        {
            Debug.Log("Not enough money!");
            return;
        }
        GameObject obj = Instantiate(
            prefabToSpawn,
            rightHand.position,
            rightHand.rotation
        );
    }
}