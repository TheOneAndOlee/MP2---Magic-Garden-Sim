using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Homework : MonoBehaviour
{
    private HashSet<GameObject> students = new HashSet<GameObject>();
    public int StudentCount => students.Count;
    public int maxStudents = 5;
    private bool registered = false;
    public TextMeshPro s;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Zombie"))
            return;

        if (students.Count >= maxStudents)
            return;

        if (students.Add(other.gameObject))
        {
            HomeworkManager.Instance.AddStudent();
            s.text = students.Count +"/5";
            if (!registered)
            {
                HomeworkManager.Instance.AddSchool();
                registered = true;
            }

            Destroy(other.gameObject);
        }
    }
}