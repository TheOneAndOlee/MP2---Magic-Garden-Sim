using UnityEngine;
using TMPro;

public class ZT_CloudTutorial : MonoBehaviour
{
    private enum Step
    {
        BuildUnderCloud = 0,
        WaitForRain = 1,
        GrowAndHarvest = 2,
        Done = 3
    }

    private Step currentStep;

    [SerializeField] private TMP_Text signLabel;

    [Header("Step Text")]
    [TextArea][SerializeField] private string buildUnderCloudText = "[Cloud Tutorial 1/3]: This is a rain cloud. It always drifts along the same path, and its rain only falls on the ground beneath it. Watch where it goes, then build a Planting Bed under its path and sow a seed.";
    [TextArea][SerializeField] private string waitForRainText = "[Cloud Tutorial 2/3]:  Seed planted! Now wait for the cloud to pass over. If the soil never turns dark, your bed is outside the cloud's path. Try building another one closer to where the rain falls.";
    [TextArea][SerializeField] private string growAndHarvestText = "[Cloud Tutorial 3/3]: The rain soaked your bed! See how the soil turned dark? Your flower grows whenever the soil is wet. Harvest 2 flowers in total to unlock the next part of the garden.";
    [TextArea][SerializeField] private string doneText = "[Cloud Tutorial] Complete! You have learned to garden with the clouds! The School is now open. Go there to train graduates, then use them to build Rain Machines that help you grow more flowers.";

    private void OnEnable()
    {
        ZT_Bed.FlowerPlanted += OnFlowerPlanted;
        ZT_Bed.RainedOn += OnBedRainedOn;
    }

    private void OnDisable()
    {
        ZT_Bed.FlowerPlanted -= OnFlowerPlanted;
        ZT_Bed.RainedOn -= OnBedRainedOn;
    }

    void Start()
    {
        SetStep(Step.BuildUnderCloud);
    }

    private void SetStep(Step step)
    {
        currentStep = step;

        if (signLabel == null) return;

        switch (step)
        {
            case Step.BuildUnderCloud:
                signLabel.text = buildUnderCloudText;
                break;
            case Step.WaitForRain:
                signLabel.text = waitForRainText;
                break;
            case Step.GrowAndHarvest:
                signLabel.text = growAndHarvestText;
                break;
            case Step.Done:
                signLabel.text = doneText;
                break;
        }
    }

    private void OnFlowerPlanted(ZT_Bed bed, ZT_Flower flower)
    {
        if (currentStep == Step.BuildUnderCloud)
        {
            SetStep(Step.WaitForRain);
        }
    }

    // Sandbox 2：有花的花圃第一次被雨淋到，给正反馈
    private void OnBedRainedOn(ZT_Bed bed)
    {
        if (currentStep < Step.GrowAndHarvest && bed.HasFlower)
        {
            SetStep(Step.GrowAndHarvest);
        }
    }

    // 由进度管理器 RainMachine 阶段的 onEnter 调用
    public void FinishTutorial()
    {
        SetStep(Step.Done);
    }
}