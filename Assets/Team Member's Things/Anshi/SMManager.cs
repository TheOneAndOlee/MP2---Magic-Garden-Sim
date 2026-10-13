
using TMPro;
using UnityEngine;

public class SMManager : MonoBehaviour
{
    public static SMManager Instance;

    public int totalSMCreated = 0;
    public TextMeshPro smText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public bool TryCreateSM()
    {
        if (HomeworkManager.Instance == null)
            return false;

        if (HomeworkManager.Instance.homeworkNeeded < 10)
            return false;

        HomeworkManager.Instance.homeworkNeeded -= 10;
        HomeworkManager.Instance.RefreshUI();
        totalSMCreated++;

        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (smText != null)
            smText.text = "Study Materials Created: " + totalSMCreated;
    }
    public void RefreshUI()
{
    UpdateUI();
}
}
