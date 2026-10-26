using UnityEngine;
using TMPro;

public class ZT_Message : MonoBehaviour
{
    public static ZT_Message Current { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageLabel;
    [SerializeField] private float duration = 3f;

    [Header("Follow Player")]
    [SerializeField] private float distance = 1.2f;
    [SerializeField] private float heightOffset = -0.25f;
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private string prefix = "<b>Hint</b>\n";

    private float timer;
    private Transform cam;

    void Awake()
    {
        if (Current != null && Current != this)
        {
            Destroy(gameObject);
            return;
        }
        Current = this;
    }

    void Start()
    {
        panel.SetActive(false);
    }

    public static void Show(string message)
    {
        if (Current == null)
        {
            Debug.Log(message);
            return;
        }
        Current.Display(message);
    }

    private void Display(string message)
    {
        messageLabel.text = prefix + message;
        panel.SetActive(true);
        timer = duration;
        SnapToPlayer();
    }


    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                panel.SetActive(false);
            }
        }
    }
    //private void LateUpdate()
  //  {
   //     if (!panel.activeSelf) return;
   //     transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), followSpeed * Time.deltaTime);
  //      FacePlayer();
  //  }

    private void SnapToPlayer()
    {
        if (!FindCamera()) return;
        transform.position = GetTargetPosition();
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
        if(forward.sqrMagnitude > 0.001f)
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
