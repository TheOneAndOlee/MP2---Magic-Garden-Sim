using UnityEngine;

public class Barricade : MonoBehaviour
{
    public int health = 100;

    void Update()
    {
        // When health drops to 0 or below, the barricade disappears
        if (health <= 0) 
        {
            Destroy(gameObject);
        }
    }
}