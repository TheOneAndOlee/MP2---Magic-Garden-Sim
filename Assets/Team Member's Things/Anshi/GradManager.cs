using TMPro;
using UnityEngine;

public class GradManager : MonoBehaviour
{
    public static GradManager Instance;

    public int totalGraduated = 0;
    public TextMeshPro graduatedText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddGraduate()
    {
        totalGraduated++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (graduatedText != null)
            graduatedText.text = "Graduated Students: " + totalGraduated;
    }
}