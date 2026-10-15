using UnityEngine;

public class popups : MonoBehaviour
{
    public GameObject bookstorepopup;
    public GameObject lecturehpopup;
    public GameObject studyhpopup;
    public GameObject finalpopup;
    public GameObject dininghpopup;

    private readonly Vector3 popupRotation = new Vector3(-90f, 0f, 0f);

    private bool foundbookstore = false;
    private bool foundlectureh = false;
    private bool foundstudyh = false;
    private bool foundfinal = false;
    private bool founddiningh = false;

    private void SpawnPopup(
        GameObject target,
        GameObject prefab,
        Vector3 offset)
    {
        if (target == null || prefab == null)
            return;

        Vector3 spawnPosition = target.transform.position + offset;

        GameObject popup = Instantiate(
            prefab,
            spawnPosition,
            Quaternion.Euler(popupRotation)
        );

        // Keep the popup in world space instead of inheriting target rotation.
        popup.transform.SetParent(null, true);

        // Follow the target's position without inheriting its rotation.
        PopupFollower follower = popup.AddComponent<PopupFollower>();
        follower.target = target.transform;
        follower.offset = offset;
        follower.worldRotation = Quaternion.Euler(popupRotation);

        Destroy(popup, 10f);
    }

    void Update()
    {
        if (!foundbookstore)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("bookstore");
            if (obj != null)
            {
                SpawnPopup(obj, bookstorepopup, new Vector3(3f, 3f, 0f));
                foundbookstore = true;
            }
        }

        if (!foundlectureh)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("LectureH");
            if (obj != null)
            {
                SpawnPopup(obj, lecturehpopup, new Vector3(3f, 3f, 0f));
                foundlectureh = true;
            }
        }

        if (!foundstudyh)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("studyh");
            if (obj != null)
            {
                SpawnPopup(obj, studyhpopup, new Vector3(3f, 3f, 0f));
                foundstudyh = true;
            }
        }

        if (!foundfinal)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("final");
            if (obj != null)
            {
                SpawnPopup(obj, finalpopup, new Vector3(3f, 3f, 0f));
                foundfinal = true;
            }
        }

        if (!founddiningh)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("diningj");
            if (obj != null)
            {
                SpawnPopup(obj, dininghpopup, new Vector3(3f, 3f, 0f));
                founddiningh = true;
            }
        }
    }
}

public class PopupFollower : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Quaternion worldRotation;

    void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = target.position + offset;
        transform.rotation = worldRotation;
    }
}