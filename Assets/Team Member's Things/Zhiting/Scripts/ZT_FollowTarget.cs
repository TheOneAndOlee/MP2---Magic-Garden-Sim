using UnityEngine;

public class ZT_FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;            
    [SerializeField] private Vector3 positionOffset;     
    [SerializeField] private Vector3 rotationOffset;     

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.TransformPoint(positionOffset);
        transform.rotation = target.rotation * Quaternion.Euler(rotationOffset);
    }
}