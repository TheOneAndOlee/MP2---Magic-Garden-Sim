using UnityEngine;
using UnityEngine.InputSystem;

public class OsmondPrefabController : MonoBehaviour
{
    [SerializeField]
    private GameObject EverythingPrefab;

    void Start()
    {
        if (GradManager.Instance != null)
        {
            GradManager.Instance.FiftyGrads.AddListener(UnlockResearch);
        }

        EverythingPrefab.SetActive(false);
    }

    private void UnlockResearch()
    {
        EverythingPrefab.SetActive(true);
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
            }

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                // 1. Instantly activate the arsenal prefab without waiting for 50 grads
                UnlockResearch();

                // 2. Find your teammate's inventory script and bypass the point cap 
                // so you can afford all the upgrades in the menu
                PlayerInventory inventory = FindAnyObjectByType<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.Inventory[SimResource.ResourceType.ResearchPoints] = 5000f;
                }

                Debug.Log("Cheat Activated: Arsenal unlocked and 5000 points added for upgrades!");
            }
        }
    }
}