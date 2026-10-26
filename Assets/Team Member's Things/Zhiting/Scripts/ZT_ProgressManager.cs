using UnityEngine;
using UnityEngine.Events;
using TMPro;


public enum ZT_Stage
{
    TableTutorial = 0,
    BuildTutorial = 1,
    CloudGarden = 2,
    RainMachine = 3,

    GardenComplete = 4,
}


public class ZT_ProgressManager : MonoBehaviour
{
    public static ZT_ProgressManager Current { get; private set; }
    public ZT_Stage CurrentStage { get; private set;  } = ZT_Stage.TableTutorial;
    public int totalHarvested { get; private set; }
    public int totalDemolished { get; private set; }

    [System.Serializable]
    public class StageInfo
    {
        public ZT_Stage stage;
        [TextArea] public string goalText;
        public UnityEvent onEnter;
    }

    [SerializeField] private StageInfo[] stages;
    [SerializeField] private TMP_Text goalLabel;
    [SerializeField] private int flowersToUnlockRainmachine = 2;

    void Awake()
    {
        if (Current != null && Current != this)
        {
            Destroy(gameObject);
            return;
        }
        Current = this;
    }

    public bool HasReached(ZT_Stage stage)
    {
        return CurrentStage >= stage;
    }

    public void ReportHarvest(int amount)
    {
        totalHarvested += amount;
        CheckThresholds();
    }

    public void ReportDemolish()
    {
        totalDemolished++;
        CheckThresholds();
    }

    private void CheckThresholds()
    {
        bool advanced = true;
        while (advanced)
        {
            advanced = false;
            switch (CurrentStage)
            {
                case ZT_Stage.TableTutorial:
                    if (totalHarvested >= 1)
                    {
                        EnterStage(ZT_Stage.BuildTutorial);
                        advanced = true;
                    }
                    break;

                case ZT_Stage.BuildTutorial:
                    if(totalDemolished >= 1)
                    {
                        EnterStage(ZT_Stage.CloudGarden);
                        advanced = true;
                    }
                    break;

                case ZT_Stage.CloudGarden:
                    if(totalHarvested >= flowersToUnlockRainmachine)
                    {
                        EnterStage(ZT_Stage.RainMachine);
                        advanced = true;
                    }
                    break;

            }
        }
    }

    void Start()
    {
        EnterStage(CurrentStage);
    }

    private void EnterStage(ZT_Stage stage)
    {
        CurrentStage = stage;

        foreach (StageInfo info in stages)
        {
            if (info.stage == stage)
            {
                if (goalLabel != null)
                {
                    goalLabel.text = info.goalText;
                }
                info.onEnter.Invoke();
            }
        }

        Debug.Log($"[Progress] Entered stage: {stage}");
    }

    
}
