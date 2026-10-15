using TMPro;
using UnityEngine;

public class ZT_ResourceDisplay : MonoBehaviour
{
    public TMP_Text text;

    void Update()
    {
        string display = "";

        foreach (ZT_ResourceType type in System.Enum.GetValues(typeof(ZT_ResourceType)))
        {
            display += type + ": " + ZT_Resources.Instance.GetResources(type).ToString("0") + "\n";
        }

        text.text = display;
    }
}