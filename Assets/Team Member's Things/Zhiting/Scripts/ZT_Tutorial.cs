using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZT_Tutorial : MonoBehaviour
{
    public string tutorialID = "Bed";       
    [TextArea] public string message = "Maintenance Log #4127: ...";
    public float triggerDistance = 1.5f;    
    public float showDuration = 20f;         

    public GameObject popup;               
    public TMP_Text popupText;            

    private static HashSet<string> shown = new HashSet<string>();

    private Transform player;
    private float timer;
    private bool showing = false;

    void Start()
    {
        player = Camera.main.transform;     
        popupText.text = message;
        popup.SetActive(false);
    }

    void Update()
    {
        if (!showing)
        {
           if (!shown.Contains(tutorialID) && Vector3.Distance(player.position, transform.position) < triggerDistance){
                shown.Add(tutorialID);
                popup.SetActive(true);
                showing = true;
                timer = 0;
            }

            return;
        }

        popup.transform.LookAt(player);
        popup.transform.Rotate(0, 180, 0); 


        timer += Time.deltaTime;
        if (timer >= showDuration)
        {
            popup.SetActive(false);
            showing = false;              
        }
    }
}