using UnityEngine;
using TMPro;

public class ZT_BuildTutorial : MonoBehaviour
{
    private enum Step
    {
        EnterBuild = 0,
        PlaceBed = 1,
        Demolish = 2,
        Done = 3
    }

    private Step currentStep;

    [SerializeField] private TMP_Text signLabel;

    [Header("Step Text")]
    [TextArea][SerializeField] private string enterBuildText = "[Build Tutorial 1/3]: You can build now! Press A to enter build mode.";
    [TextArea][SerializeField] private string placeBedText = "[Build Tutorial 2/3]: Point at the ground. A green preview means you can build there, red means you can't. Press B to switch between buildings, and pull the trigger to place a Planting Bed.";
    [TextArea][SerializeField] private string demolishText = "[Build Tutorial 3/3]: Nice work, your first Planting Bed! Placed something in the wrong spot? Point at the bed and press X on your left controller to remove it. You will get your resources back.";
    [TextArea][SerializeField] private string doneText = "[Build Tutorial] Complete! Head to the other table to start the Cloud Tutorial.";

    private void OnEnable()
    {
        ZT_BuildPlacer.EnterBuild += OnBuildModeEntered;
        ZT_BuildPlacer.PlaceBed += OnBedPlaced;
        ZT_BuildPlacer.Demolish += OnBedDemolished;
    }

    private void OnDisable()
    {
        ZT_BuildPlacer.EnterBuild -= OnBuildModeEntered;
        ZT_BuildPlacer.PlaceBed -= OnBedPlaced;
        ZT_BuildPlacer.Demolish -= OnBedDemolished;
    }

    void Start()
    {
        SetStep(Step.EnterBuild);
    }

    private void SetStep(Step step)
    {
        currentStep = step;

        if (signLabel == null) return;

        switch (step)
        {
            case Step.EnterBuild:
                signLabel.text = enterBuildText;
                break;
            case Step.PlaceBed:
                signLabel.text = placeBedText;
                break;
            case Step.Demolish:
                signLabel.text = demolishText;
                break;
            case Step.Done:
                signLabel.text = doneText;
                break;
        }
    }

    private void OnBuildModeEntered(ZT_BuildPlacer placer)
    {
        if (currentStep == Step.EnterBuild)
        {
            SetStep(Step.PlaceBed);
        }
    }

    private void OnBedPlaced(ZT_BuildPlacer placer)
    {
        if (currentStep == Step.PlaceBed)
        {
            SetStep(Step.Demolish);
        }
    }

    private void OnBedDemolished(ZT_BuildPlacer placer)
    {
        if (currentStep == Step.Demolish)
        {
            SetStep(Step.Done);
        }
    }
}