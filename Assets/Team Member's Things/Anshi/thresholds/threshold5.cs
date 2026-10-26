
using UnityEngine;

public class threshold5 : MonoBehaviour
{
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
        unlockCardInstance = Instantiate(unlockCardPrefab);
        unlockCardInstance.gameObject.SetActive(false);
        UpdateMaterial();
    }

    void Update()
    {
        
        GameObject bookstore = GameObject.FindGameObjectWithTag("diningj");

        if (!isUnlocked &&
            bookstore != null )
        {
            unlockCardInstance.ShowCard("School");
            isUnlocked = true;
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
