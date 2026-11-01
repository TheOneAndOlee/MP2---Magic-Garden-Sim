using UnityEngine;

public class ResearchBarrier : MonoBehaviour
{
    [Header("Teammate Integration")]
    public ResearchProgression researchManager;
    public string requiredResearchID = "bow";

    void Update()
    {
        if (researchManager != null)
        {
            // As soon as the research returns true, disable this wall
            if (researchManager.IsResearched(requiredResearchID))
            {
                Debug.Log("Bow researched! Removing physical barrier.");
                gameObject.SetActive(false);
            }
        }
    }
}