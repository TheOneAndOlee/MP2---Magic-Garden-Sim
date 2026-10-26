
using UnityEngine;
using TMPro;
using System.Collections;

public class reveal : MonoBehaviour
{
    public float displayTime = 3f;
    public float distance = 1.5f;

    private Transform playerCamera;
    private Coroutine hideCoroutine;

    public void ShowCard(string buildingName)
    {
        if (Camera.main == null)
            return;

        playerCamera = Camera.main.transform;

        transform.position =
            playerCamera.position +
            playerCamera.forward * distance;

        Vector3 cameraRotation = playerCamera.eulerAngles;
        transform.rotation = Quaternion.Euler(-90f, cameraRotation.y, cameraRotation.z);    


        gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideCard());
    }

    IEnumerator HideCard()
    {
        yield return new WaitForSeconds(displayTime);
        gameObject.SetActive(false);
    }
}
