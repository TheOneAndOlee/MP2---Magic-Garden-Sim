using UnityEngine;

public class BarrierHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 500;
    private int currentHealth;

    [Header("Destruction Effects")]
    public AudioClip destructionSound;
    public GameObject destructionParticles;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            TriggerDestruction();
        }
    }

    private void TriggerDestruction()
    {
        // Play the destruction sound at the barricade's location
        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, transform.position);
        }

        // Spawn the destruction particle effect
        if (destructionParticles != null)
        {
            Instantiate(destructionParticles, transform.position, transform.rotation);
        }

        // Delete the barricade
        Destroy(gameObject);
    }
}