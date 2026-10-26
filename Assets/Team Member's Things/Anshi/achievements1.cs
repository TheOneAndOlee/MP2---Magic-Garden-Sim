
using UnityEngine;

public class achievements1 : MonoBehaviour
{
    public HomeworkManager hwManager;
    public int studentThreshold = 50;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    private Renderer objectRenderer;
    private bool isUnlocked = false;
    public reveal unlockCardPrefab;

    private reveal unlockCardInstance;
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        unlockCardInstance = Instantiate(unlockCardPrefab);
        unlockCardInstance.gameObject.SetActive(false);
        UpdateMaterial();
    }

    void Update()
    {
        if (!isUnlocked &&
            hwManager.studentCount >= studentThreshold)
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
