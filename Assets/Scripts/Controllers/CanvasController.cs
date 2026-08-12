using System.Collections;
using UnityEngine;
using Valve.VR.Extras;
using UXF;
using TableMemory;


public class CanvasController : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;
    public GameObject controller;
    private Collider controllerCollider;

    public ExperimentManager experimentManager;
        
    public GameObject researcherDisplay;

    public GameObject instructionsRoot;
    public GameObject buttonsRoot;
    public GameObject calibrateButton;
    public GameObject backButton;
    public GameObject nextButton;
    public GameObject continueButton;
    public GameObject instructions;

    public int numInstruction = 0;
    public int totInstructions;
    private string[] instructionList;

    public Session session;

    public CanvasState canvasState;

    public ExperimentLocation experimentLocation;
    public CanvasInstructions canvasInstructions;
    public GhostController ghostController;

    private bool adjustedLocation;
    public ContextManager contextManager;
    public ControllerHints controllerHints;

    private void Awake()
    {
        adjustedLocation = false;

        canvasInstructions.CollectInstructions();
        ShowInstructions(true);

        laserPointer = GameObject.FindGameObjectWithTag("Laser").GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerClick += this.PointerClick;
        controllerCollider = controller.GetComponent<Collider>();

        SetCanvasState(CanvasState.Init);
    }

    private void Start()
    {
    }

    public void SetupAfterInitialised()
    {
        adjustedLocation = false;

        canvasInstructions.SetCustomInstructions();
        SetCanvasState(CanvasState.Intro);
    }

    public void SetCanvasState(CanvasState state)
    {
        continueButton.SetActive(false);
        nextButton.SetActive(false);
        calibrateButton.SetActive(false);
        backButton.SetActive(false);

        if (laserPointer != null && laserPointer.holder != null)
        {
            laserPointer.holder.SetActive(true);
            laserPointer.pointer.SetActive(true);
        }

        canvasState = state;
        switch (state)
        {
            case CanvasState.Init:
                instructions.SetActive(true);
                canvasInstructions.NewInstructions(canvasInstructions.initInstructionControllers, this);
                continueButton.SetActive(false);
                experimentManager.gameObject.SetActive(false);
                break;

            case CanvasState.Intro:
                nextButton.SetActive(true);
                canvasInstructions.NewInstructions(canvasInstructions.introInstructionControllers, this);
                break;

            case CanvasState.Baseline:
                nextButton.SetActive(true);
                canvasInstructions.NewInstructions(canvasInstructions.baselineInstructionControllers, this);
                break;

            case CanvasState.Practice:
                nextButton.SetActive(true);
                canvasInstructions.NewInstructions(canvasInstructions.practiceInstructionControllers, this);
                break;

            case CanvasState.Experiment:
                nextButton.SetActive(true);
                canvasInstructions.NewInstructions(canvasInstructions.experimentInstructionControllers, this);
                break;

            case CanvasState.Break:
                experimentManager.DisableExperiment();
                ShowInstructions(true);

                continueButton.SetActive(true);
                calibrateButton.SetActive(true);

                controllerCollider.enabled = true;

                canvasInstructions.NewInstructions(canvasInstructions.breakInstructionsControllers, this);
                break;

            case CanvasState.Pause:
                experimentManager.DisableExperiment();
                ShowInstructions(true);

                continueButton.SetActive(true);
                calibrateButton.SetActive(true);

                controllerCollider.enabled = true;

                canvasInstructions.NewInstructions(canvasInstructions.pauseInstructionsControllers, this);
                break;

            case CanvasState.Halfway:
                canvasInstructions.NewInstructions(canvasInstructions.halfwayInstructionsControllers, this);

                StartCoroutine(CalibrateButtonDelaySequence());
                break;

            case CanvasState.Finished:
                canvasInstructions.NewInstructions(canvasInstructions.finishedInstructionsControllers, this);
                break;

            case CanvasState.ContextChange:
                canvasInstructions.NewInstructions(canvasInstructions.contextSwitchInstructionsControllers, this);
                break;
        }
    }

    public void SetInstruction(string text)
    {
        canvasInstructions.SetCurrentInstruction(text);
    }

    private void PointerClick(object sender, PointerEventArgs e)
    {
        switch (e.target.name)
        {
            case "Next":
                NextClick(); break;
            case "Back":
                BackClick(); break;
            case "Continue":
                ContinueClick(); break;
            case "Calibrate":
                break;
        }
    }

    public void EnableCalibration(bool canSkip)
    {
        calibrateButton.SetActive(true);
        nextButton.SetActive(canSkip);
    }

    public void LastInstruction()
    {
        nextButton.SetActive(false);
        continueButton.SetActive(true);
    }

    public void NextClick()
    {
        if (!adjustedLocation)
        {
            // First time next button is clicked
            experimentManager.gameObject.SetActive(true);
            experimentLocation.AdjustExperimentHeight();
            adjustedLocation = true;
        }
        calibrateButton.SetActive(false);
        backButton.SetActive(true);
        canvasInstructions.NextInstruction(this);
    }

    public void FirstInstruction()
    {
        backButton.SetActive(false);
    }

    public void BackClick()
    {
        calibrateButton.SetActive(false);
        nextButton.SetActive(true);
        continueButton.SetActive(false);
        canvasInstructions.PreviousInstruction(this);
    }

    public void ContinueClick() {
        switch (canvasState) {
            case CanvasState.Intro:
                SetCanvasState(CanvasState.Baseline);
                break;

            default:
                //*** Start experiment ***//
                ghostController.MakeTransparent();
                controllerCollider.enabled = true;
                    
                researcherDisplay.SetActive(true);
                ShowInstructions(false);    

                experimentManager.EnableExperiment();
                break;
        }
    }

    public void ContextChange()
    {
        StartCoroutine(SwitchContextRoutine());
    }

    IEnumerator SwitchContextRoutine()
    {
        contextManager.EnableHints();
        contextManager.ChangeLocation();

        yield return new WaitUntil(contextManager.FinishedContextSwitch);

        controllerHints.gameObject.SetActive(false);
        canvasInstructions.NextInstruction(this);
    }

    IEnumerator CalibrateButtonDelaySequence()
    {
        yield return new WaitForSeconds(15);
        calibrateButton.SetActive(true);
    }

    public void ShowInstructions(bool show = true)
    {
        instructionsRoot.SetActive(show);
        buttonsRoot.SetActive(show);
    }

    public void EndOfBlock(BlockType blockType)
    {
        // Turn off Experiment, Turn on Canvas
        experimentManager.DisableExperiment();
        ShowInstructions(true);

        // Set Controller to UI mode
        laserPointer.holder.SetActive(true);
        laserPointer.pointer.SetActive(true);
        controllerCollider.enabled = false;
        ghostController.MakeOpaque();

        switch (blockType)
        {
            case BlockType.Baseline:
                // Set Canvas State to Practice
                SetCanvasState(CanvasState.Practice);
                break;

            case BlockType.Practice:
                // Set Canvas State to Experiment
                SetCanvasState(CanvasState.Experiment);
                break;

            case BlockType.Experiment:
                EndOfExperimentBlock();
                break;
        }
    }

    private void EndOfExperimentBlock()
    {
        // Disable controls
        backButton.SetActive(false);
        nextButton.SetActive(false);

        // Block 1 = baseline
        // Block 2 = practice
        int experimentalBlockNum = session.currentBlockNum - 2;
        int totalExperimentalBlocks = session.blocks.Count - 2;

        if (experimentalBlockNum == totalExperimentalBlocks)
        {
            SetCanvasState(CanvasState.Finished);
        }
        else
        {
            SetCanvasState(CanvasState.Break);
        }
    }

}
public enum CanvasState
{
    Init,
    Intro,
    Baseline,
    Practice,
    Experiment,
    Break,
    Halfway,
    Finished,
    ContextChange,
    Pause
}
