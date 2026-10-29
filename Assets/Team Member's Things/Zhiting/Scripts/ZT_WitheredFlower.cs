using UnityEngine;

public class ZT_WitheredFlower : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ZT_Shears>() != null)
        {
            Destroy(gameObject);
        }
    }
}