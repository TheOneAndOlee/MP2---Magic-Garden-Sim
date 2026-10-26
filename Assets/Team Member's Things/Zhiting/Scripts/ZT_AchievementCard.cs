using UnityEngine;
using TMPro;

public class ZT_AchievementCard : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private TMP_Text bodyLabel;
    [SerializeField] private float duration = 6f;

    [Header("Follow Player")]
    [SerializeField] private float distance = 3f;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private float followSpeed = 6f;

    private float timer;
    private Transform cam;

    void Start()
    {
        panel.SetActive(false);
    }


    public void Show(string content)
    {
        string[] parts = content.Split('|');
        titleLabel.text = parts[0].Trim();
        bodyLabel.text = parts.Length > 1 ? parts[1].Trim() : "";

        panel.SetActive(true);
        timer = duration;

        if (FindCamera())
        {
            transform.position = GetTargetPosition();
            FacePlayer();
        }
    }


    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                panel.SetActive(false);
            }
        }
    }

    void LateUpdate()
    {
        if (!panel.activeSelf) return;
        transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), followSpeed * Time.deltaTime);
        FacePlayer();
    }

    private Vector3 GetTargetPosition()
    {
        if (!FindCamera()) return transform.position;
        Vector3 forward = cam.forward;
        forward.y = 0;
        forward.Normalize();
        return cam.position + forward * distance + Vector3.up * heightOffset;
    }

    private void FacePlayer()
    {
        if (!FindCamera()) return;
        Vector3 forward = transform.position - cam.position;
        forward.y = 0;
        if (forward.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }

    private bool FindCamera()
    {
        if (cam == null && Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        return cam != null;
    }
}