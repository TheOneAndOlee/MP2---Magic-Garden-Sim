using UnityEngine;
using UnityEngine.Events;

public class GameStartTrigger : MonoBehaviour
{
    [Header("Teammate Integration")]
    public ResearchProgression researchManager;
    public string requiredResearchID = "bow";

    [Header("Game Start Action")]
    public UnityEvent onGameStart;

    private bool gameHasStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameHasStarted)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            bool isResearched = false;

            if (researchManager != null)
            {
                isResearched = researchManager.IsResearched(requiredResearchID);
            }

            if (isResearched)
            {
                gameHasStarted = true;
                Debug.Log("Player crossed the barricade with bows unlocked. Starting game!");

                if (onGameStart != null)
                {
                    onGameStart.Invoke();
                }
            }
            else
            {
                Debug.Log("Player crossed the barricade, but bow research is missing.");
            }
        }
    }
}