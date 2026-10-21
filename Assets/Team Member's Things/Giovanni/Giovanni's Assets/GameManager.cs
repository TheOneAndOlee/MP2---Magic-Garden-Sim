using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Currency Settings")]
    public int currentCurrency = 0;
    public TextMeshProUGUI currencyText; 

    [Header("Progression")]
    public int globalUpgradeTier = 1;

    [Header("Invasion Settings")]
    public float alienSpawnRate = 3.0f; 
    public int aliensPerSpawn = 1; 
    public float alienSpeedMultiplier = 1.0f; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCurrencyDisplay();
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        UpdateCurrencyDisplay();
    }

    public bool SpendCurrency(int amount)
    {
        if (currentCurrency >= amount)
        {
            currentCurrency -= amount;
            UpdateCurrencyDisplay();
            return true; 
        }
        
        return false; 
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = "Current Currency: $" + currentCurrency;
        }
    }

    public void IncreaseAlienSpawns()
    {
        aliensPerSpawn *= 4;
        
        alienSpawnRate = Mathf.Max(0.5f, alienSpawnRate - 0.2f);

        // Reverted to the flat 15% increase per escalation
        alienSpeedMultiplier += 0.15f; 
        
        Debug.Log("Escalation triggered! Spawning " + aliensPerSpawn + " aliens at " + alienSpeedMultiplier + "x speed!");
    }
}