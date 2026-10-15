using UnityEngine;

public class ZT_PathFollower : MonoBehaviour
{
    public float speed = 1f;
    private Transform[] pathPoints;
    private int currentNumber = 0;

    void Start()
    {
       pathPoints =  FindFirstObjectByType<ZT_CloudPath>().pathPoints;
       transform.position = pathPoints[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentNumber >= pathPoints.Length)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentNumber].position, speed*Time.deltaTime);
        if (Vector3.Distance(transform.position, pathPoints[currentNumber].position) < 0.05f)
        {
            currentNumber++;
        }
    }
}
