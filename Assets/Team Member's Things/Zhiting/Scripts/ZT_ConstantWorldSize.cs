using UnityEngine;

public class ZT_ConstantWorldSize : MonoBehaviour
{
    [SerializeField] private float worldScale = 0.001f;

    void Start()
    {
        Vector3 parentScale = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
        transform.localScale = new Vector3(
            worldScale / parentScale.x,
            worldScale / parentScale.y,
            worldScale / parentScale.z);
    }
}