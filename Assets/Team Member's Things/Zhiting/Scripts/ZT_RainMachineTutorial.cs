using UnityEngine;
using TMPro;

public class ZT_RainMachineTutorial : MonoBehaviour
{
    private enum Step
    {
        TrainGraduates = 0,
        CatchRain = 1,
        SurroundWithBeds = 2,
        Done = 3
    }

    private Step currentStep;

    [SerializeField] private TMP_Text signLabel;

    [Header("Step Text")]
    [TextArea][SerializeField] private string trainText = "[Rain Machine Tutorial 1/3]: Go to the School and train graduates. Every 3 graduates let you run 1 Rain Machine. Then come back and build it under the cloud's path.";
    [TextArea][SerializeField] private string catchText = "[Rain Machine Tutorial 2/3]: Rain Machine built! Wait for the cloud to pass over it and watch the water level rise. If it never fills, demolish it and build it closer to the rain.";
    [TextArea][SerializeField] private string surroundText = "[Rain Machine Tutorial 3/3]: It caught the rain! A Rain Machine waters the four beds directly up, down, left and right of it. Build a Planting Bed on each side and sow them.";
    [TextArea][SerializeField] private string doneText = "[Garden Complete] Your Rain Machine is watering four beds on its own. You are now a Professor of Dream Flowers!";

    private void OnEnable()
    {
        ZT_RainMachine.Placed += OnMachinePlaced;
        ZT_RainMachine.RainCollected += OnRainCollected;
    }

    private void OnDisable()
    {
        ZT_RainMachine.Placed -= OnMachinePlaced;
        ZT_RainMachine.RainCollected -= OnRainCollected;
    }

    void Start()
    {
        SetStep(Step.TrainGraduates);
    }

    private void SetStep(Step step)
    {
        currentStep = step;

        if (signLabel == null) return;

        switch (step)
        {
            case Step.TrainGraduates:
                signLabel.text = trainText;
                break;
            case Step.CatchRain:
                signLabel.text = catchText;
                break;
            case Step.SurroundWithBeds:
                signLabel.text = surroundText;
                break;
            case Step.Done:
                signLabel.text = doneText;
                break;
        }
    }

    private void OnMachinePlaced(ZT_RainMachine machine)
    {
        if (currentStep == Step.TrainGraduates)
        {
            SetStep(Step.CatchRain);
        }
    }


    private void OnRainCollected(ZT_RainMachine machine)
    {
        if (currentStep < Step.SurroundWithBeds)
        {
            SetStep(Step.SurroundWithBeds);
        }
    }


    public void FinishTutorial()
    {
        SetStep(Step.Done);
    }
}