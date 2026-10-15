
using UnityEngine;
using TMPro;

public class studymat : MonoBehaviour
{
    public float productionInterval = 2f;
    public TextMeshPro smText;

    public int smCreated = 0;

    private float timer = 0f;

    private void Start()
    {
        UpdateSMText();
    }

    private void Update()
    {
        if (SMManager.Instance == null ||
            HomeworkManager.Instance == null)
            return;

        timer += Time.deltaTime;

        if (timer >= productionInterval)
        {
            if (SMManager.Instance.TryCreateSM())
            {
                smCreated++;
                UpdateSMText();
            }

            timer = 0f;
        }
    }

    private void UpdateSMText()
    {
        if (smText != null)
            smText.text = "SM Created: " + smCreated;
    }
}
