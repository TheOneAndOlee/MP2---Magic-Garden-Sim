using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GradManager : MonoBehaviour
{
    public UnityEvent FiftyGrads;

    public static GradManager Instance;
            public TextMeshPro nextg;

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
        nextg.text = "Next Goal:\nCreate more graduates to run research labs and arsenal!";

        if (totalGraduated >= 50)
        {
            FiftyGrads.Invoke();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (graduatedText != null)
            graduatedText.text = "Graduated Students: " + totalGraduated;
    }
}