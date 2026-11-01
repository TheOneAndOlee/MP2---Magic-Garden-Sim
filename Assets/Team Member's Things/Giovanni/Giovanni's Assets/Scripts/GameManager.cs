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
    public int maxAliensPerSpawn = 40;
    public float alienSpeedMultiplier = 1.0f;

    [Header("Game State")]
    public bool wavesActive = false;

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
        wavesActive = false;
    }

    public void BeginAlienWaves()
    {
        if (!wavesActive)
        {
            wavesActive = true;
            Debug.Log("Barricade crossed! Waves are now ACTIVE.");
        }
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

        aliensPerSpawn = Mathf.Min(aliensPerSpawn, maxAliensPerSpawn);

        alienSpawnRate = Mathf.Max(0.5f, alienSpawnRate - 0.2f);
        alienSpeedMultiplier += 0.15f;

        Debug.Log("Escalation triggered! Spawning " + aliensPerSpawn + " aliens at " + alienSpeedMultiplier + "x speed!");
    }
}