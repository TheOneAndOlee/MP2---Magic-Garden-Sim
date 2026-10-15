using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class Building : MonoBehaviour
{
    #region Variables and their Getters & Setters

    [Header("Building Health")]
    public float health = 200f;
    public float maxHealth = 200f;

    [Tooltip("How much health the building loses each second")]
    [SerializeField, Min(0)]
    private float decayRate = 4f;

    [Header("Building References")]
    [SerializeField] private ParticleSystem smokeFX;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Building_Data building_Data;

    private PlayerInventory playerInventory;
    private GridManager gridManager;
    private Vector2Int buildingCell;
    private bool isDestroying = false;

    private int displayedHealth = -1;
    private float displayedMaximum = -1f;

    public Building_Data Data
    {
        get => building_Data;
    }

    public bool IsAlive
    {
        get => !isDestroying && health > 0f;
    }

    public float EfficacyMultiplier
    {
        get
        {
            float healthRatio = 0f;

            if (maxHealth > 0f)
            {
                healthRatio = Mathf.Clamp01(health / maxHealth);
            }

            // Full health gives the full bonus. Near zero health gives 70% of it.
            return Mathf.Lerp(0.7f, 1f, healthRatio);
        }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        health = Mathf.Clamp(health, 0f, maxHealth);

        if (healthText == null)
        {
            healthText = GetComponentInChildren<TMP_Text>(true);
        }

        if (smokeFX == null)
        {
            smokeFX = GetComponentInChildren<ParticleSystem>(true);
        }

        RefreshPresentation();
    }

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    private void OnEnable()
    {
        RegisterPlacement();
    }

    private void OnDisable()
    {
        ReleasePlacement();
    }

    private void OnDestroy()
    {
        ReleasePlacement();
    }

    #endregion

    #region Health Methods

    public void Tick(float seconds)
    {
        if (isDestroying)
        {
            return;
        }

        float healthLost = Mathf.Max(0f, decayRate) * Mathf.Max(0f, seconds);
        health = Mathf.Clamp(health - healthLost, 0f, maxHealth);
        RefreshPresentation();

        if (health <= 0f)
        {
            isDestroying = true;
            ReleasePlacement();
            Destroy(gameObject);
        }
    }

    public void RestoreHealth(float amount)
    {
        if (!IsAlive || amount <= 0f)
        {
            return;
        }

        health = Mathf.Clamp(health + amount, 0f, maxHealth);
        RefreshPresentation();
    }

    public void RefreshPresentation()
    {
        UpdateHealthDisplay();
        UpdateSmokeEffect();
    }

    private void UpdateHealthDisplay()
    {
        if (healthText == null)
        {
            return;
        }

        int currentHealth = Mathf.CeilToInt(health);

        // Only change the text when the displayed number actually changes.
        if (displayedHealth != currentHealth || displayedMaximum != maxHealth)
        {
            healthText.SetText("Health: {0} / {1}", currentHealth, Mathf.CeilToInt(maxHealth));
            displayedHealth = currentHealth;
            displayedMaximum = maxHealth;
        }
    }

    private void UpdateSmokeEffect()
    {
        if (smokeFX == null)
        {
            return;
        }

        bool shouldSmoke = IsAlive && health <= maxHealth * 0.4f;

        if (shouldSmoke && !smokeFX.isPlaying)
        {
            smokeFX.Play(true);
        }

        if (!shouldSmoke && (smokeFX.isPlaying || smokeFX.particleCount > 0))
        {
            smokeFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    #endregion

    #region Placement Methods

    public void Initialize(Building_Data data, PlayerInventory inventory, GridManager grid, Vector2Int cell)
    {
        ReleasePlacement();

        building_Data = data;
        playerInventory = inventory;
        gridManager = grid;
        buildingCell = cell;

        RegisterPlacement();
    }

    private void RegisterPlacement()
    {
        if (!IsAlive)
        {
            return;
        }

        if (gridManager != null && !gridManager.TryRegister(buildingCell, gameObject))
        {
            return;
        }

        StatBuilding buildingEffect = GetComponent<StatBuilding>();

        if (playerInventory != null && buildingEffect != null)
        {
            playerInventory.Register(buildingEffect);
        }
    }

    private void ReleasePlacement()
    {
        if (gridManager != null)
        {
            gridManager.Unregister(buildingCell, gameObject);
        }

        StatBuilding buildingEffect = GetComponent<StatBuilding>();

        if (playerInventory != null && buildingEffect != null)
        {
            playerInventory.Unregister(buildingEffect);
        }
    }

    #endregion
}
