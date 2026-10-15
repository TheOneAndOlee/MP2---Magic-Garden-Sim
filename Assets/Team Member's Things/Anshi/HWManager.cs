
using TMPro;
using UnityEngine;

public class HomeworkManager : MonoBehaviour
{
    public static HomeworkManager Instance;

    public float homeworkNeeded = 0;
    public int studentCount = 0;
    public int activeSchools = 0;

    public TextMeshPro homeworkText;
    public TextMeshPro totalStudents;

    public float homeworkInterval = 10f;
    public float homeworkIncrease = 5f;

    private float timer = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        Homework[] schools = FindObjectsByType<Homework>();

        int eligibleSchools = 0;

        foreach (Homework school in schools)
        {
            if (school.StudentCount >= school.maxStudents)
            {
                eligibleSchools++;
            }
        }

        if (eligibleSchools == 0)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= homeworkInterval)
        {
            homeworkNeeded += homeworkIncrease * eligibleSchools;
            timer = 0f;
            UpdateUI();
        }
    }

    public void AddStudent()
    {
        studentCount++;
        UpdateUI();
    }

    public void AddSchool()
    {
        activeSchools++;
        UpdateUI();
    }

    public void RemoveSchool()
    {
        activeSchools = Mathf.Max(0, activeSchools - 1);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (homeworkText != null)
            homeworkText.text = "Homework Needed: " + homeworkNeeded;

        if (totalStudents != null)
            totalStudents.text = "Total Students: " + studentCount;
    }
    public void RefreshUI()
    {
        UpdateUI();
    }
}
