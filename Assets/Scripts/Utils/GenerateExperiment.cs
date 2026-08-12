using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UXF;
using Random = UnityEngine.Random;
namespace TableMemory
{
    public class GenerateExperiment : MonoBehaviour
    {
        public ResearcherCanvasController researcherCanvasController;
        public StimulusCollectionManager experimentalStimulusCollectionManager;
        public StimulusCollectionManager practiceStimulusCollectionManager;
        public ResponseManager responseManager;

        [SerializeField] int numBaselineTrials = 8;
        [SerializeField] int numPracticeBlocks = 1;
        [SerializeField] int numExperimentalBlocks = 10;
        [SerializeField] int numPracticeStimuli = 10;
        [SerializeField] int numStudyTrials = 1;
        [SerializeField] List<int> numStudyStimuli = new List<int>() { 8, 9, 10, 11, 12 };
        [SerializeField] float percentStudyHighlights = 0.5f;
        [SerializeField] int testTrialsPerStudyItem = 2;

        public void Generate(Session session)
        {
            session.settings.SetValue("n_baseline_trials", numBaselineTrials);
            session.settings.SetValue("n_practice_blocks", numPracticeBlocks);
            session.settings.SetValue("n_practice_stimuli", numPracticeStimuli);
            session.settings.SetValue("n_experimental_blocks", numExperimentalBlocks);

            session.settings.SetValue("n_study_stimuli", string.Join(", ", numStudyStimuli));
            session.settings.SetValue("n_study_highlights", percentStudyHighlights);

            session.settings.SetValue("n_study_trials", numStudyTrials);
            session.settings.SetValue("n_test_trials", testTrialsPerStudyItem);


            //*** BASELINE BLOCK ***//

            Block baselineBlock = session.CreateBlock(numBaselineTrials);
            baselineBlock.settings.SetValue("block_type", BlockType.Baseline);

            for (int i = 0; i < numBaselineTrials; i++)
            {
                baselineBlock.GetRelativeTrial(i + 1).settings.SetValue("baseline_response", i % 2 == 0 ? StimulusMemory.New : StimulusMemory.Old);
                baselineBlock.GetRelativeTrial(i + 1).settings.SetValue("trial_type", TrialType.Baseline);
            }
            baselineBlock.trials.Shuffle();
            DataManager.RecordSessionSetting("baseline_blocks", 1);


            //*** Trial Settings ***//


            //*** PRACTICE BLOCK ***//

            // Only use practice words for the practice block

            // Limit trials according to settings
            /*int numberOfPracticeTrials = Mathf.Min(trialSettings.Count, maxPracticeTrials);
            session.settings.SetValue("n_practice_trials", numberOfPracticeTrials);

            Block practiceBlock = session.CreateBlock(numberOfPracticeTrials);
            practiceBlock.settings.SetValue("block_type", TrialType.Practice);

            for (int i = 0; i < numberOfPracticeTrials; i++)
            {
                Dictionary<string, object> trial = trialSettings[i];
            }
            practiceBlock.trials.Shuffle();*/



            //*** EXPERIMENTAL BLOCKS ***//

            int numberOfTaskBlocks = numExperimentalBlocks + numPracticeBlocks;
            List<int> possibleStudyStimuli = numStudyStimuli.ToList();

            Block[] experimentalBlocks = new Block[numberOfTaskBlocks];
            for (int blockIndex = 0; blockIndex < numberOfTaskBlocks; blockIndex++)
            {
                int numStudyStimuliForTrial;
                if (blockIndex == 0)
                {
                    // Use max number of stimuli in first block - for equal learning across participants
                    numStudyStimuliForTrial = numPracticeStimuli;
                }
                else
                {
                    // Pick random number of study stimuli
                    numStudyStimuliForTrial = possibleStudyStimuli[Random.Range(0, possibleStudyStimuli.Count)];
                    possibleStudyStimuli.Remove(numStudyStimuliForTrial);

                    if (possibleStudyStimuli.Count == 0) possibleStudyStimuli = numStudyStimuli.ToList();
                }
                
                int numHighlights = (int)(numStudyStimuliForTrial * percentStudyHighlights);
                int numTestTrials = (int)(numStudyStimuliForTrial * testTrialsPerStudyItem);
                int numBaselineTestTrials = 0;
                int trialsInBlock = numStudyTrials + numTestTrials + numBaselineTestTrials;

                Debug.Log("Block " + blockIndex + ": " + numStudyStimuliForTrial + " stimuli");
                Debug.Log("\tGenerating " + trialsInBlock + " trials");

                Block newBlock = new Block((uint)trialsInBlock, session);
                newBlock.settings.SetValue("block_type", blockIndex < numPracticeBlocks ? BlockType.Practice : BlockType.Experiment);

                //--- Study trials ---//

                // Choose a set of random objects to study
                StimulusCollectionManager stimuliManager;
                if (blockIndex < numPracticeBlocks)
                {
                    stimuliManager = practiceStimulusCollectionManager;
                }
                else
                {
                    stimuliManager = experimentalStimulusCollectionManager;
                }
                List<ObjectController> studyStimuli = stimuliManager.GetRandomUnpickedStimuli(numStudyStimuliForTrial);

                int trialIndex = 0;
                for (trialIndex = 0; trialIndex < numStudyTrials; trialIndex += 1)
                {
                    Debug.Log("Study " + trialIndex);
                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("trial_type", TrialType.Study);
                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("settings", new StudyTrialSetting(studyStimuli, numHighlights));

                    // Switch contexts every other trial
                    // First switch on first trial so we can take a record of how long it takes
                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("location_switch", blockIndex % 2 == 1);
                }

                // --- Baseline -- //
                for (trialIndex = trialIndex; trialIndex < numStudyTrials + numBaselineTestTrials; trialIndex++)
                {
                    Debug.Log("Baseline " + trialIndex);

                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("baseline_response", trialIndex % 2 == 0 ? StimulusMemory.New : StimulusMemory.Old);
                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("trial_type", TrialType.Baseline);
                }

                //--- Test trials ---//

                TestTrialSetting[] testTrialSettings = GenerateTestTrials(stimuliManager, studyStimuli, numTestTrials);

                int testTrialIndex = 0;
                for (trialIndex = trialIndex; trialIndex < trialsInBlock; trialIndex += 1)
                {
                    Debug.Log("Test " + trialIndex);

                    // Test trial
                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("trial_type", TrialType.Test);

                    TestTrialSetting trialSettings = testTrialSettings[testTrialIndex];
                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("settings", trialSettings);
                    
                    newBlock.GetRelativeTrial(trialIndex + 1).settings.SetValue("location_switch", false);

                    testTrialIndex += 1;
                }

                Debug.Log("Unpicked: " + stimuliManager.NumUnpickedStimuli());
                // Don't shuffle, test object list is shuffled before being returned, and we want the study trial to be first in the block
                experimentalBlocks[blockIndex] = newBlock;
            }

            DataManager.RecordSessionSetting("experimental_blocks", numExperimentalBlocks);
            DataManager.RecordSessionSetting("practice_blocks", numPracticeBlocks);
            DataManager.RecordSessionSetting("practice_stimuli", numPracticeStimuli);
            DataManager.RecordSessionSetting("baseline_trials", numBaselineTrials);
            DataManager.RecordSessionSetting("possible_study_stimuli", string.Join(", ", numStudyStimuli));
            DataManager.RecordSessionSetting("max_study_stimuli", numStudyStimuli.Max());
            DataManager.RecordSessionSetting("min_study_stimuli", numStudyStimuli.Max());

            DataManager.RecordSessionSetting("highlights_per_study_stimulus", percentStudyHighlights);
            DataManager.RecordSessionSetting("tests_per_study_stimulus", testTrialsPerStudyItem);

            responseManager.Initialise();
        }

        private TestTrialSetting[] GenerateTestTrials(StimulusCollectionManager stimuliManager, List<ObjectController> studyStimuli, int totalTestStimuli)
        {
            List<TestTrialSetting> testSettings = new List<TestTrialSetting>();

            // Trials for "old" test objects 
            for (int i = 0; i < Math.Min(studyStimuli.Count, totalTestStimuli); i++)
            {
                testSettings.Add(new TestTrialSetting(studyStimuli[i], StimulusMemory.Old));
            }
            Debug.Log("Created " +  testSettings.Count + " old test trials");

            // Create list of "new" test objects
            int numNewTestStimuli = totalTestStimuli - testSettings.Count;
            Debug.Log("Creating " + numNewTestStimuli + " new test trials");

            List<ObjectController> newTestStimuli = stimuliManager.GetRandomUnpickedStimuli(numNewTestStimuli);

            // Trials for "new" test objects
            foreach (ObjectController stimulus in newTestStimuli)
            {
                testSettings.Add(new TestTrialSetting(stimulus, StimulusMemory.New));
            }

            TestTrialSetting[] testSettingsArray = testSettings.ToArray();
            ShuffleArray(testSettingsArray);

            return testSettingsArray;
        }

        //for shuffle number from array
        private void ShuffleArray(object[] array)
        {
            int p = array.Length;
            for (int n = p - 1; n > 0; n--)
            {
                int r = Random.Range(0, p);
                object t = array[r];
                array[r] = array[n];
                array[n] = t;
            }
        }
    }

    public enum ResponseLocations
    {
        Left,
        Right
    }

    public enum BlockType
    {
        Baseline,
        Practice,
        Experiment
    }

    public enum TrialType 
    {
        Baseline,
        Study,
        Test
    }

    public enum StimulusMemory
    {
        Old,
        New,
        None
    }
}

