using UnityEngine;

public class ZT_DreamRibbon : MonoBehaviour
{
    public int pointCount = 60;
    public float height = 2f;
    public float radius = 0.3f;
    public float turns = 3f;
    public float speed = 30f;

    private LineRenderer line;
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = pointCount;

        float step = height / (pointCount - 1);
        float angleStep = turns * Mathf.PI * 2 / (pointCount - 1);

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * angleStep;
            Vector3 point = new Vector3(Mathf.Cos(angle) * radius, i * step, Mathf.Sin(angle) * radius);
            line.SetPosition(i, point);

        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
