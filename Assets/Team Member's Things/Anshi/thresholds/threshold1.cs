
using UnityEngine;

public class threshold1 : MonoBehaviour
{
    public spawn spawnScript;
    public int unlockThreshold = 10;

    public Material lockedMaterial;
    public Material unlockedMaterial;

    private Renderer objectRenderer;
    private bool isUnlocked = false;

    public reveal unlockCardPrefab;

    private reveal unlockCardInstance;

    void Start()
    {
        objectRenderer = GetComponentInChildren<Renderer>();

        if (spawnScript == null)
        {
            spawnScript = GetComponent<spawn>();
        }
        unlockCardInstance = Instantiate(unlockCardPrefab);
        unlockCardInstance.gameObject.SetActive(false);
        UpdateMaterial();
    }

    void Update()
    {
        if (!isUnlocked &&
            (MoneyManager.Instance.money > unlockThreshold ||
             spawnScript.unlocked1))
        {
            unlockCardInstance.ShowCard("School");
            isUnlocked = true;
            spawnScript.unlocked1 = true;
            UpdateMaterial();
        }
    }

   void UpdateMaterial()
{
    if (objectRenderer == null)
        return;

    Material[] materials = objectRenderer.materials;

    Material materialToUse = isUnlocked
        ? unlockedMaterial
        : lockedMaterial;

    for (int i = 0; i < materials.Length; i++)
    {
        materials[i] = materialToUse;
    }

    objectRenderer.materials = materials;
}
}
