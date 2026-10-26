using UnityEngine;

public class ZT_IntroDescent : MonoBehaviour
{
    public Transform rig;
    public Transform startPoint;
    public Transform endPoint;
    public float duration = 10f;
    public GameObject[] disableDuringIntro;
    private float timer;
    private bool finished = false;

    void Start()
    {
        rig.position = startPoint.position;
        foreach (GameObject obj in disableDuringIntro)
        {
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (finished) return;
        timer += Time.deltaTime;
        float t = timer / duration;
        float smoothT = Mathf.SmoothStep(0, 1, t);
        rig.position = Vector3.Lerp(startPoint.position, endPoint.position, smoothT);
        if (t>=1)
        {
            finished = true;
            foreach (GameObject obj in disableDuringIntro)
            {
                obj.SetActive(true);
            }
        }
    }
}
