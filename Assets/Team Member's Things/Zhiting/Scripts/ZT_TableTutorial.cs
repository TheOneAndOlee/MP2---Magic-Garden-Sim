using UnityEngine;
using TMPro;

public class ZT_TableTutorial : MonoBehaviour
{
    private enum Step
    {
        Sow = 0,
        Grow = 1,
        Water = 2,
        Harvest = 3,
        Done = 4
    }

    private Step currentStep;

    [SerializeField] private TMP_Text signLabel;

    [Header("Step Texts")]
    [TextArea][SerializeField] private string sowText = "Tilt the seed jar to sow a seed into the planting bed.";
    [TextArea][SerializeField] private string growText = "Your seed has sprouted. Watch it grow.";
    [TextArea][SerializeField] private string waterText = "The flower has stopped growing. It needs water. Tilt the watering can over it.";
    [TextArea][SerializeField] private string harvestText = "The dream has bloomed. Touch it with the sickle to harvest. You will get a flower and some seeds.";
    [TextArea][SerializeField] private string doneText = "Well done. Now let's learn how to build.";

    void OnEnable()
    {
        ZT_Bed.FlowerPlanted += OnFlowerPlanted;
        ZT_Flower.Stalled += OnFlowerStalled;
        ZT_Flower.Bloomed += OnFlowerBloomed;
    }

    void OnDisable()
    {
        ZT_Bed.FlowerPlanted -= OnFlowerPlanted;
        ZT_Flower.Stalled -= OnFlowerStalled;
        ZT_Flower.Bloomed -= OnFlowerBloomed;
    }

    void Start()
    {
        SetStep(Step.Sow);
    }

    private void SetStep(Step step)
    {
        currentStep = step;

        if (signLabel == null) return;

        switch (step)
        {
            case Step.Sow:
                signLabel.text = sowText;
                break;
            case Step.Grow:
                signLabel.text = growText;
                break;
            case Step.Water:
                signLabel.text = waterText;
                break;
            case Step.Harvest:
                signLabel.text = harvestText;
                break;
            case Step.Done:
                signLabel.text = doneText;
                break;
        }
    }

    private void OnFlowerPlanted(ZT_Bed bed, ZT_Flower flower)
    {
        if (currentStep == Step.Sow)
        {
            SetStep(Step.Grow);
        }
    }

    private void OnFlowerStalled(ZT_Flower flower)
    {
        if (currentStep == Step.Grow)
        {
            SetStep(Step.Water);
        }
    }

    private void OnFlowerBloomed(ZT_Flower flower)
    {
        if (currentStep < Step.Harvest)
        {
            SetStep(Step.Harvest);
        }
    }

    public void FinishTutorial()
    {
        SetStep(Step.Done);
    }
}