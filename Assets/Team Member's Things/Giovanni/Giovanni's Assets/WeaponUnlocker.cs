using UnityEngine;

public class WeaponUnlocker : MonoBehaviour
{
    [Header("Weapon to Unlock")]
    public GameObject laserPointerItem; // The physical laser pointer sitting on your menu table

    [Header("Cost Settings")]
    public int missileStrikeCost = 500; 
    
    // Replace this variable with wherever your game actually stores its money/points
    public int currentCurrency = 0; 

    void Start()
    {
        // Ensure the laser pointer starts completely hidden and disabled
        if (laserPointerItem != null)
        {
            laserPointerItem.SetActive(false);
        }
    }

    void Update()
    {
        // Continuously check if you have enough money. 
        // If you do, and the weapon is still hidden, unlock it.
        if (currentCurrency >= missileStrikeCost && laserPointerItem != null && !laserPointerItem.activeSelf)
        {
            UnlockWeapon();
        }
    }

    public void UnlockWeapon()
    {
        // Makes the laser pointer suddenly appear on the table, ready to grab
        laserPointerItem.SetActive(true);
        
        // Optional: Add a sound effect or particle pop here so the player notices it appeared
    }

    // Call this method from your enemy death scripts or score scripts
    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
    }
}