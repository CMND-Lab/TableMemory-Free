using System.Collections;
using UnityEngine;
using UXF;

namespace TableMemory
{
    public class ExperimentManager : MonoBehaviour
    {
        public Session session;

        public ResearcherCanvasController researcherCanvasController;
        
        [SerializeField] TaskController taskController;
        [SerializeField] CanvasController canvasController;
        [SerializeField] ResponseManager responseManager;

        public GameObject experiment;
        public ContextManager contextManager;
        public ControllerHints controllerHints;

        [SerializeField] ExperimentLocation experimentLocation;
        private bool recordedLocations = false;

        private void Awake()
        {
            controllerHints.gameObject.SetActive(false);
            DisableExperiment();
        }

        public void EnableExperiment()
        {
            experiment.SetActive(true);
            taskController.ResetTrial();
            contextManager.Initialise();

            // Trial hasn't started yet
            if (UseHints())
            {
                controllerHints.ShowTaskHints((TrialType)Session.instance.NextTrial.settings.GetObject("trial_type"));
            }
            
            if ((TrialType)session.NextTrial.settings.GetObject("trial_type") == TrialType.Test)
            {
                responseManager.ShowResponses(true);
                responseManager.ShowResponseLabels(true);
            }
            else
            {
                responseManager.ShowResponses(false);
                responseManager.ShowResponseLabels(false);
            }
        }

        public void DisableExperiment()
        {
            experiment.SetActive(false);
        }

        IEnumerator EndOfExperiment()
        {
            Debug.Log("Finalizing Session");
            yield return new WaitForSeconds(2.0f);
            yield return new WaitForSeconds(2.0f);
            session.End();
        }

        private bool UseHints(Trial trial = null)
        {
            if (trial == null)
            {
                if (session.NextTrial == null)
                {
                    return false;
                }
                trial = Session.instance.NextTrial;
            }

            BlockType blockType = (BlockType)trial.settings.GetObject("block_type");
            TrialType trialType = (TrialType)trial.settings.GetObject("trial_type");

            return blockType == BlockType.Practice;
        }

        public void StartOfTrial(Trial trial) {
            BlockType blockType = (BlockType)session.CurrentBlock.settings.GetObject("block_type");
            TrialType trialType = (TrialType)trial.settings.GetObject("trial_type");
            Debug.Log(blockType + " block: " + trialType + " trial " + trial.numberInBlock);

            DataManager.RecordContext(trial, contextManager);

            experimentLocation.RecordObjectPositions(trial);

            if (UseHints(trial))
            {
                taskController.RunTrial(trial, true);
            }
            else
            {
                controllerHints.gameObject.SetActive(false);
                taskController.RunTrial(trial, false);
            }

            // Researcher display
            researcherCanvasController.setAbsoluteTrial(session.currentTrialNum + 1);
            researcherCanvasController.setTrialText(trial.numberInBlock + 1, session.CurrentBlock.trials.Count);
        }

        public void EndOfTrial(Trial trial) {
            taskController.ResetTrial();

            if (trial != session.LastTrial)
            {
                BlockType blockType = (BlockType)session.CurrentBlock.settings.GetObject("block_type");
                TrialType trialType = (TrialType)trial.settings.GetObject("trial_type");

                if (blockType != BlockType.Baseline && trialType != TrialType.Baseline)
                {
                    bool locationSwitch = trial.settings.GetBool("location_switch");
                    trial.result["switch_block"] = locationSwitch;

                    // Change location contexts
                    if (locationSwitch)
                    {
                        StartCoroutine(ContextSwitch(trial));
                    }
                    // Time delay between study-test
                    else if (trialType == TrialType.Study)
                    {
                        contextManager.NoSwitchDelay(this, trial);
                        if (trial.block.number <= 2)
                        {
                            controllerHints.ShowTimeDelayHints();
                        }
                    }
                    // Reset hints if necessary for next trial
                    else if (UseHints())
                    {
                        controllerHints.ShowTaskHints((TrialType)Session.instance.NextTrial.settings.GetObject("trial_type"));
                    }
                }


                if ((TrialType)session.NextTrial.settings.GetObject("trial_type") == TrialType.Test)
                {
                    responseManager.ShowResponses(true);
                    responseManager.ShowResponseLabels(true);
                }
                else
                {
                    responseManager.ShowResponses(false);
                    responseManager.ShowResponseLabels(false);
                }
            }
        }

        IEnumerator ContextSwitch(Trial trial)
        {
            DisableExperiment();
            contextManager.ChangeLocation(trial);
            yield return new WaitUntil(contextManager.FinishedContextSwitch);
            EnableExperiment();

            // Turn off instructions
            canvasController.ShowInstructions(false);
        }

        public void StartOfBlock(Block block)
        {
            BlockType trialType = (BlockType)block.settings.GetObject("block_type");
            string trialTypeText = trialType.ToString().ToUpper().Substring(1).ToLower();

            //*** RESEARCHER DISPLAY ***//
            researcherCanvasController.setExperimentText(trialTypeText);
            researcherCanvasController.setBlockText(block.number - 1, session.blocks.Count);
        }

        public void EndOfBlock(Block block) // Is called at the end of each block of trials via the UXF Event system
        {
            //Make sure task has been reset
            taskController.ResetTrial();

            controllerHints.gameObject.SetActive(false);

            // Get block type
            BlockType trialType = (BlockType)session.CurrentTrial.settings.GetObject("block_type");

            Debug.Log("End of " + trialType.ToString() + " block");
            canvasController.EndOfBlock(trialType);

            if (session.GetBlock(session.blocks.Count).number == block.number)
            {
                StartCoroutine(EndOfExperiment());          
            }
        }
    }
}


