using UnityEngine;

public class BarrierHealth : MonoBehaviour
{
    [Header("Barrier Stats")]
    public int maxHealth = 500;
    private int currentHealth;

    [Header("Effects")]
    public GameObject destructionEffect;
    public AudioClip breakSound;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        
        // Add an AudioSource if one is missing but a sound is assigned
        if (audioSource == null && breakSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            BreakBarrier();
        }
    }

    private void BreakBarrier()
    {
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, transform.rotation);
        }

        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        Destroy(gameObject);
    }
}