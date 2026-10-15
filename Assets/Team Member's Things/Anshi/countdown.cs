using UnityEngine;
using TMPro;
using System.Collections;

public class countdown : MonoBehaviour
{
    public TextMeshPro countdownText;

    private float timeRemaining;
    private int materialsPerSession;

    public void Initialize(float duration, int materials)
    {
        timeRemaining = duration;
        materialsPerSession = materials;

        StartCoroutine(SessionTimer());
    }

    IEnumerator SessionTimer()
    {
        while (timeRemaining > 0)
        {
            if (countdownText != null)
            {
                countdownText.text =
                    "Study Session Time left: " + Mathf.CeilToInt(timeRemaining) + "s";
            }

            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        for (int i = 0; i < materialsPerSession; i++)
        {
            if (SMManager.Instance == null)
                break;

            if (!SMManager.Instance.TryCreateSM())
                break;
        }

        Destroy(gameObject);
    }
}