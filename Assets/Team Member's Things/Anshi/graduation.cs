using UnityEngine;
using TMPro;
public class graduation : MonoBehaviour
{
    public float graduationInterval = 2f;
    private float timer = 0f;
    public TextMeshPro s;
    public int grads=0;
    private void Update()
    {
        if (SMManager.Instance == null ||
            GradManager.Instance == null)
            return;

        if (SMManager.Instance.totalSMCreated < 50)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= graduationInterval)
        {
            SMManager.Instance.totalSMCreated -= 50;
            grads+=1;
            s.text = "Graduates: "+grads;
            SMManager.Instance.RefreshUI();

            GradManager.Instance.AddGraduate();

            timer = 0f;
        }
    }
}