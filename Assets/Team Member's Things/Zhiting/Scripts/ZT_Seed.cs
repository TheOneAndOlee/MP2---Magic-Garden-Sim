using UnityEngine;

public class ZT_Seed : MonoBehaviour
{
    public float lifetime = 5f;
    private bool resolved = false;

    void Start()
    {
        Invoke(nameof(Expire), lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (resolved) return;


        if (collision.gameObject.GetComponentInParent<ZT_Bed>() != null) return;

        if (collision.gameObject.GetComponentInParent<ZT_WateringCan>() != null) return;
        if (collision.gameObject.GetComponent<ZT_Seed>() != null) return;

        ZT_Message.Show("Seeds only grow in an empty Planting Bed. The seed went back into your jar.");
        Refund();
    }


    private void Expire()
    {
        if (resolved) return;
        Refund();
    }


    public void Refund()
    {
        if (resolved) return;
        resolved = true;
        ZT_Resources.Instance.AddResources(ZT_ResourceType.Seed, 1);
        Destroy(gameObject);
    }

    public void Consume()
    {
        resolved = true;
        Destroy(gameObject);
    }
}