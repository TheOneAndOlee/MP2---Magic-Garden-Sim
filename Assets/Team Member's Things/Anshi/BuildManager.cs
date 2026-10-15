using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public GameObject selectedPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }
}