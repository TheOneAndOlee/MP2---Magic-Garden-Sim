using UnityEngine;
using TMPro;
public class spawn : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform rightHand;
    public int cost = 100;
    public bool unlocked1= false;
    public bool unlocked2= false;
    public bool unlocked3=false;
    public bool unlocked4=false;
    public GameObject lechallPrefab;
    private GameObject lechallInstance;
    public GameObject stPrefab;
    private GameObject stInstance;
    public GameObject fePrefab;
    private GameObject feInstance;
    public GameObject dhPrefab;
    private GameObject dhInstance;
    
    public void Spawn()
    {
        
        if (prefabToSpawn.name == "city_hall")
        {
            if (unlocked1)
            {
                if (!unlocked1)
                {
                    unlocked1= true;
                }
                if (!MoneyManager.Instance.SpendMoney(cost))
            {
                Debug.Log("Not enough money!");
                return;
            }
                GameObject obj = Instantiate(
                prefabToSpawn,
                rightHand.position,
                rightHand.rotation);
            }else {
            
                dhInstance = Instantiate(
                    dhPrefab,
                    transform.position + new Vector3(-2, 0, 0),
                    Quaternion.Euler(0, 0, 90)
                );

                Destroy(dhInstance, 5f);
            
                }
        } else if (prefabToSpawn.name == "school") {
            if (unlocked2)
            {
                if (!unlocked2)
                {
                    unlocked2= true;
                }

                if (!MoneyManager.Instance.SpendMoney(cost))
                {
                    Debug.Log("Not enough money!");
                    return;
                }
                GameObject obj = Instantiate(
                prefabToSpawn,
                rightHand.position,
                rightHand.rotation);
            } else 
                {
            
                stInstance = Instantiate(
                    stPrefab,
                    transform.position + new Vector3(-2, 0, 0),
                    Quaternion.Euler(0, 0, 90)
                );

                Destroy(stInstance, 5f);
            
                }
        } else if (prefabToSpawn.name == "classbuilding")
        {
            if (unlocked3)
            {
                if (!unlocked3)
                {
                    unlocked3= true;
                }
                if (!MoneyManager.Instance.SpendMoney(cost))
            {
                Debug.Log("Not enough money!");
                return;
            }
                GameObject obj = Instantiate(
                prefabToSpawn,
                rightHand.position,
                rightHand.rotation);
            } else 
                {
            
                lechallInstance = Instantiate(
                    lechallPrefab,
                    transform.position + new Vector3(-2, 0, 0),
                    Quaternion.Euler(0, 0, 90)
                );

                Destroy(lechallInstance, 5f);
            
                }
        } else if (prefabToSpawn.name == "computer_store")
        {
            if (unlocked4)
            {
                if (!unlocked4)
                {
                    unlocked4= true;
                }
                if (!MoneyManager.Instance.SpendMoney(cost))
            {
                Debug.Log("Not enough money!");
                return;
            }
                GameObject obj = Instantiate(
                prefabToSpawn,
                rightHand.position,
                rightHand.rotation);
            }else {
            
                feInstance = Instantiate(
                    fePrefab,
                    transform.position + new Vector3(-2, 0, 0),
                    Quaternion.Euler(0, 0, 90)
                );

                Destroy(feInstance, 5f);
            
                }
        }
        else
        {
            if (!MoneyManager.Instance.SpendFlower(cost))
            {
                Debug.Log("Not enough Flower!");
                return;
            }
            GameObject obj = Instantiate(
            prefabToSpawn,
            rightHand.position,
            rightHand.rotation
        );
        }
        
    }
}