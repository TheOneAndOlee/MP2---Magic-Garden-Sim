
using UnityEngine;
using TMPro;
public class threshold3 : MonoBehaviour
{
    public spawn spawnScript;
    public int unlockThreshold = 10;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    private Renderer objectRenderer;
    private bool isUnlocked = false;
        public TextMeshPro nextg;

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
        
        GameObject bookstore = GameObject.FindGameObjectWithTag("bookstore");

        if (!isUnlocked &&
            bookstore != null )
        {
            unlockCardInstance.ShowCard("School");

            isUnlocked = true;
            spawnScript.unlocked3 = true;
            nextg.text = "Next Goal:\nDrag students to lecture to create homeworks!";

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
