using UnityEngine;
using UnityEngine.InputSystem;

public class OsmondPrefabController : MonoBehaviour
{
    [SerializeField]
    private GameObject EverythingPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if (GradManager.Instance != null)
        {
            GradManager.Instance.FiftyGrads.AddListener(UnlockResearch);
        }

        EverythingPrefab.SetActive(false);
    }

    // Update is called once per frame
    private void UnlockResearch()
    {
        EverythingPrefab.SetActive(true);
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
                GradManager.Instance.AddGraduate();
            }
        }
    }
}
