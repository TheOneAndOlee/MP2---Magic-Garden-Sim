using UnityEngine;

public class ZT_Seed : MonoBehaviour
{
    public float lifetime = 2f;
    private bool resolved = false;

    void Start()
    {
        Invoke(nameof(Expire), lifetime);
    }

    private void Expire()
    {
        if (resolved) return;
        ZT_Message.Show("Seeds only grow in an empty Planting Bed. The seed went back into your jar.");
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