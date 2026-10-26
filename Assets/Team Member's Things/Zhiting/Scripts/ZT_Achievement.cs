using UnityEngine;

public class ZT_Achievements : MonoBehaviour
{
    [System.Serializable]
    public class Achievement
    {
        public string title;
        [TextArea] public string description;
        public GameObject trophy;
        [HideInInspector] public bool unlocked;
    }

    [Header("0: Dream Harvester  1: Rain Maker  2: Full Bloom")]
    [SerializeField] private Achievement[] achievements = new Achievement[3];

    [Header("Card")]
    [SerializeField] private ZT_AchievementCard achievementCard;  

    [Header("Conditions")]
    [SerializeField] private int harvestGoal = 10;
    [SerializeField] private int rainMachineGoal = 3;
    [SerializeField] private int bloomGoal = 6;
    [SerializeField] private float checkInterval = 0.5f;

    private int rainMachinesBuilt = 0;
    private float timer;

    void OnEnable()
    {
        ZT_RainMachine.Placed += OnRainMachinePlaced;
    }

    void OnDisable()
    {
        ZT_RainMachine.Placed -= OnRainMachinePlaced;
    }

    void Start()
    {
        foreach (Achievement a in achievements)
        {
            if (a.trophy != null) a.trophy.SetActive(false);
        }
    }

    private void OnRainMachinePlaced(ZT_RainMachine machine)
    {
        rainMachinesBuilt++;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < checkInterval) return;
        timer = 0;

        if (ZT_ProgressManager.Current != null && ZT_ProgressManager.Current.totalHarvested >= harvestGoal)
        {
            Unlock(0);
        }

        if (rainMachinesBuilt >= rainMachineGoal)
        {
            Unlock(1);
        }

        if (!achievements[2].unlocked && CountBloomingFlowers() >= bloomGoal)
        {
            Unlock(2);
        }
    }

    private int CountBloomingFlowers()
    {
        int count = 0;
        ZT_Flower[] flowers = FindObjectsByType<ZT_Flower>();
        foreach (ZT_Flower f in flowers)
        {
            if (f.bloom >= f.maxBloom) count++;
        }
        return count;
    }

    private void Unlock(int index)
    {
        if (index < 0 || index >= achievements.Length) return;
        Achievement a = achievements[index];
        if (a.unlocked) return;

        a.unlocked = true;
        if (a.trophy != null) a.trophy.SetActive(true);


        if (achievementCard != null)
        {
            achievementCard.Show("Achievement Unlocked: " + a.title + " | " + a.description);
        }
        Debug.Log("[Achievement] " + a.title);
    }
}