
using UnityEngine;

public class achievements : MonoBehaviour
{
    public GradManager hwManager;
    public int studentThreshold = 50;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    private Renderer objectRenderer;
    private bool isUnlocked = false;
        private bool isUnlocked2 = false;

    public reveal unlockCardPrefab;

    private reveal unlockCardInstance;
    public reveal unlockCardPrefab2;

    private reveal unlockCardInstance2;
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        unlockCardInstance = Instantiate(unlockCardPrefab);
        unlockCardInstance.gameObject.SetActive(false);
        unlockCardInstance2 = Instantiate(unlockCardPrefab2);
        unlockCardInstance2.gameObject.SetActive(false);
        UpdateMaterial();
    }

    void Update()
    {
        if (!isUnlocked &&
            hwManager.totalGraduated >0)
        {
                unlockCardInstance.ShowCard("School");

            isUnlocked = true;
            UpdateMaterial();
        } else if (!isUnlocked2&&hwManager.totalGraduated>=10)
        {
            unlockCardInstance2.ShowCard("School");

            isUnlocked2 = true;
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
