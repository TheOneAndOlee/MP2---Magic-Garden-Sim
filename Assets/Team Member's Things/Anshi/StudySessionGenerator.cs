using UnityEngine;
using System.Collections;

public class StudySessionGenerator : MonoBehaviour
{
    public GameObject studySessionPrefab;
    public HomeworkManager hwManager;

    public float minSpawnInterval = 30f;
    public float maxSpawnInterval = 70f;

    public float materialDelay = 10f;
    public int requiredStudents = 5;
    public int materialsPerSession = 5;
    public float spawnRange = 5f;

    void Start()
    {
        StartCoroutine(GenerateStudySessions());
    }

    IEnumerator GenerateStudySessions()
    {
        while (true)
        {
            float randomInterval = Random.Range(
                minSpawnInterval,
                maxSpawnInterval
            );

            yield return new WaitForSeconds(randomInterval);

            if (hwManager != null &&
                hwManager.studentCount >= requiredStudents)
            {
                SpawnStudySession();
            }
        }
    }

    void SpawnStudySession()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-7f, 7f),
            0f,
            Random.Range(-7f, 7f)
        );

        GameObject session = Instantiate(
            studySessionPrefab,
            randomPosition,
            Quaternion.identity
        );

        countdown timer = session.GetComponent<countdown>();

        if (timer != null)
        {
            timer.Initialize(materialDelay, materialsPerSession);
        }
        else
        {

            StartCoroutine(CreateStudyMaterials(session));
        }
    }

    IEnumerator CreateStudyMaterials(GameObject session)
        {
        yield return new WaitForSeconds(materialDelay);

        if (SMManager.Instance != null)
        {
            SMManager.Instance.TryCreateSM();
        }

        if (session != null)
        {
            Destroy(session);
        }

        }
}