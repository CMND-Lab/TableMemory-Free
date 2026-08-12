using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using Valve.VR.Extras;


namespace TableMemory
{
    public class TaskController : MonoBehaviour
    {
        public Session session;
        [SerializeField] GameObject experiment;
        [SerializeField] DataManager dataManager;
        [SerializeField] AudioController audioController;
        [SerializeField] StartingPointTrialController startingPoint;
        [SerializeField] HandGrabController handGrabController;

        public SteamVR_LaserPointer laserPointer;
        [SerializeField] ControllerHints controllerHints;
        [SerializeField] bool useControllerHints = false;

        [SerializeField] StimulusCollectionManager objectCollectionManager;
        [SerializeField] ResponseManager responseManager;

        [SerializeField] SpawnLocationsManager spawnLocationsManager;

        public TimeManager timeManager;


        [Header("Trial Settings")]
        [SerializeField] float startOrbHoldTime;
        private Trial currentTrial;
        private TrialType currentTrialType;

        [SerializeField] bool endTrial = false;
        private bool timedOutResponse = false;
        private bool forceStop = false;
        private bool correctTrialResponse;

        [Header("Study Settings")]
        public GameObject studyDisplay;
        private StudyTrialSetting studySetting;

        [SerializeField] bool studyLaserInteraction = false;
        [SerializeField] bool useStudyTimer;

        [SerializeField] int numStudyHighlights;
        [SerializeField] int numObjectsHit;
        private string previousHighlight = null;

        private List<string> hitObjects;
        private List<string> hitTimes;
        private List<ObjectController> studyObjects;
        [SerializeField] List<TileController> studyTiles;
        [SerializeField] TileController highlight;
        [SerializeField] float highlightDelay = 1.0f;

        [Header("Test Settings")]
        public GameObject testDisplay;
        [SerializeField] GameObject dropArea;

        [SerializeField] ObjectController testObject;
        [SerializeField] StimulusMemory reachObjectMemoryAnswer;
        [SerializeField] ConfirmController insideConfirmButton;
        [SerializeField] ConfirmController outsideConfirmButton;

        [SerializeField] bool objectIsPlaced;
        private List<float> pickupTimes;
        private List<float> dropTimes;
        private List<Vector3> testObjectPlacements;
        private List<float> placementTimes;

        public void Start()
        {
            spawnLocationsManager.CreateTileMap();
        }

        public void Awake()
        {
            startingPoint.SetHoldTime(startOrbHoldTime);
            ResetTrial();
        }

        public void EnableLaser(bool useLaser)
        {

            laserPointer.holder.SetActive(useLaser);
            laserPointer.pointer.SetActive(useLaser);

            if (useLaser)
            {
                insideConfirmButton.EnableClick(true);
                outsideConfirmButton.EnableClick(true);
            }
        }


        private void RecordSettingsOnFirstTrial()
        {
            session.participantDetails["start_hold_duration"] = startOrbHoldTime;
        }

        // Called at the start of each trial via UFX
        public void RunTrial(Trial trial, bool useHints = false)
        {
            if (trial.number == 1)
            {
                RecordSettingsOnFirstTrial();
            }

            currentTrial = trial;
            useControllerHints = useHints;
            Debug.Log("Using hints: " + useControllerHints);

            BlockType blockType = (BlockType)trial.settings.GetObject("block_type");
            TrialType trialType = (TrialType)trial.settings.GetObject("trial_type");
            currentTrialType = trialType;

            trial.result["trial_type"] = trialType.ToString();
            trial.result["block_type"] = blockType.ToString();

            if (blockType == BlockType.Baseline || trialType == TrialType.Baseline)
            {
                SetupBaselineTrial(trial, blockType);
            }
            else
            {
                TrialSetting trialSettings = (TrialSetting)trial.settings.GetObject("settings");

                if (trialType == TrialType.Study)
                {
                    SetupStudyTrial(trial, trialSettings);
                }
                else if (trialType == TrialType.Test)
                {
                    SetupTestTrial(trial, trialSettings);
                }
            }

            // Start trial sequence
            endTrial = false;
            StartCoroutine(TaskTrialSequence(trial));
        }

        private void SetupBaselineTrial(Trial trial, BlockType blockType)
        {
            //responseManager.ShowResponses(true);

            StimulusMemory baselineResponse = (StimulusMemory)trial.settings.GetObject("baseline_response");
            reachObjectMemoryAnswer = baselineResponse;

            responseManager.BaselineResponse(baselineResponse);
            responseManager.ShowResponseLabels(blockType != BlockType.Baseline);

            EnableLaser(false);
        }

        private void SetupStudyTrial(Trial trial, TrialSetting settings)
        {
            StudyTrialSetting trialSettings = (StudyTrialSetting) settings;
            studySetting = trialSettings;

            studyDisplay.SetActive(true);

            List<GameObject> studyStimuli = trialSettings.GetStimuliGameObjects();

            // Spawn objects at random locations on table
            studyTiles = spawnLocationsManager.CreateStudyObjects(studyStimuli);
            Debug.Log("Study Trial: " + trial.number);

            studyObjects = new List<ObjectController>();

            foreach (TileController tile in studyTiles)
            {
                ObjectController objectController = tile.GetObject().GetComponent<ObjectController>();
                if (objectController != null)
                {
                    studyObjects.Add(objectController);
                    objectController.SetLayer(8);
                    Debug.Log("\t" + objectController.GetName());
                }
            }

            trialSettings.StoreObjectLocations(studyObjects);

            numObjectsHit = 0;
            numStudyHighlights = trialSettings.GetNumHighlights();

            // Enable laser pointer
            EnableLaser(studyLaserInteraction);

            // Record settings
            DataManager.RecordStudyTrial(trial, trialSettings, studyObjects);

            previousHighlight = null;
            // Highlight a tile for the user to click
            StartCoroutine(HighlightTile(highlightDelay * 3));
        }

        private void SetupTestTrial(Trial trial, TrialSetting settings)
        {
            responseManager.ShowResponses(true);
            responseManager.ShowResponseLabels(true);
            responseManager.EnableColliders(true);

            // Disable laser pointer
            EnableLaser(false);

            TestTrialSetting trialSettings = (TestTrialSetting)settings;
            ObjectController testStimulus = trialSettings.GetStimulus();

            testObject = spawnLocationsManager.CreateTestObject(testStimulus);
            testObject.gameObject.transform.SetParent(gameObject.transform, true);
            testObject.SetLayer(8);

            reachObjectMemoryAnswer = trialSettings.GetMemoryStatus();

            Debug.Log("Test Trial: " + trial.number);
            Debug.Log("\t" + testStimulus.GetName());
            Debug.Log("\tStatus: " + reachObjectMemoryAnswer.ToString());

            // Record settings
            DataManager.RecordLocationForTrial(trial, "test_judgement", testObject);

            trial.result["trial_stimulus"] = testObject.GetName();
            trial.result["memory_status"] = trialSettings.GetMemoryStatus().ToString();

            responseManager.RecordPositions(trial);
            
            // If an old object, record the position it was in during study
            if (trialSettings.GetMemoryStatus() == StimulusMemory.Old)
            {
                Vector3 studyObjectLocation = studySetting.GetLocationForObject(testObject);
                int numObjectHighlights = studySetting.GetNumHighlightsForObject(testObject);

                trial.result["object_num_highlights"] = numObjectHighlights.ToString();
                trial.result["object_name_study"] = testObject.GetName();
                trial.result["object_location_study"] = studyObjectLocation.ToString();

                Debug.Log("\tOriginal location: " + studyObjectLocation.ToString());
            }
        }

        // Called at the end of each trial via UFX
        public void ResetTrial()
        {
            startingPoint.gameObject.SetActive(true);
            startingPoint.ResetStartOrb();
            responseManager.Reset();
            responseManager.ShowResponses(true);
            handGrabController.Reset();

            endTrial = false;
            timedOutResponse = false;
            forceStop = false;
            reachObjectMemoryAnswer = StimulusMemory.None;
            correctTrialResponse = false;
            objectIsPlaced = false;
            currentTrial = null;
            useControllerHints = false;

            numStudyHighlights = 0;
            numObjectsHit = 0;
            studyTiles = new List<TileController>();
            studyObjects = new List<ObjectController>();
            hitObjects = new List<string>();
            hitTimes = new List<string>();

            if (highlight != null) { highlight.gameObject.SetActive(false); }
            highlight = null;

            pickupTimes = new List<float>();
            dropTimes = new List<float>();
            testObjectPlacements = new List<Vector3>();
            placementTimes = new List<float>();

            spawnLocationsManager.Reset();

            if (testObject != null)
            {
                Destroy(testObject.gameObject);
                testObject = null;
            }

            studyDisplay.SetActive(false);
            dropArea.SetActive(false);

            insideConfirmButton.gameObject.SetActive(false);
            outsideConfirmButton.gameObject.SetActive(false);
        }

        public void PickupObject()
        {
            ObjectPlaced(false);
            pickupTimes.Add(Time.time);
            dropArea.SetActive(true);

            if (useControllerHints) { controllerHints.SetHint("Place the object on the table, where you remember seeing it"); }
        }

        public void DropObject()
        {
            ObjectPlaced(true);
            dropTimes.Add(Time.time);
            dropArea.SetActive(false);

            if (objectIsPlaced)
            {
                testObjectPlacements.Add(testObject.gameObject.transform.position);
                placementTimes.Add(Time.time);

                if (useControllerHints) { controllerHints.SetHint("Use the laser pointer to click the <b>CONFIRM</b> button on the back of the table"); }
            }
        }

        public void ResetObject()
        {
            ObjectPlaced(false);
            dropArea.SetActive(false);
            dropTimes.Add(Time.time);
        }

        public void ObjectPlaced(bool placed)
        {
            objectIsPlaced = placed;
            Debug.Log("Obect placed: " + objectIsPlaced);

            if (!placed)
            {
                insideConfirmButton.EnableClick(false);
                outsideConfirmButton.EnableClick(false);
            }

            EnableLaser(placed);
        }

        // Called from StartingPointTrialController when the user exits the orb during a trial
        public void ExitStartOrb()
        {
            dataManager.ExitStartOrb();
        }

        // Called from an object controller when the user clicks it with laser
        public void HitStudyObject(ObjectController hitObject)
        {
            string timeString = "time_touch_highlight_" + numObjectsHit;
            DataManager.RecordTime(timeString);

            string objectName = hitObject != null ? hitObject.GetName() : "location " + numObjectsHit;
            Debug.Log("Hit object: " + objectName);
            hitObjects.Add(objectName);
            hitTimes.Add(Time.time.ToString());

            numObjectsHit += 1;

            highlight.gameObject.SetActive(false);

            if (useControllerHints) { controllerHints.SetHint("Now move back to the start..."); }

        }

        public void ExitStudyObject(ObjectController hitObject)
        {
            hitObject.DisableLaser();

            if (useControllerHints) { controllerHints.SetHint("You'll have to interact with a few objects..."); }

            if (numObjectsHit >= numStudyHighlights)
            {
                endTrial = true;
                correctTrialResponse = numObjectsHit == numStudyHighlights;
            }
            else
            {
                StartCoroutine(HighlightTile(highlightDelay));
            }
        }

        IEnumerator HighlightTile(float delay=0.0f)
        {
            bool validHighlight = false;
            GameObject objectAtTile;
            ObjectController tileObjectController = null;
            while (!validHighlight) 
            {
                // Pick random tile from list of spawned objects
                highlight = studyTiles[UnityEngine.Random.Range(0, studyTiles.Count)];

                objectAtTile = highlight.GetObject();
                tileObjectController = objectAtTile.GetComponent<ObjectController>();

                if (studySetting.GetNumHighlightsForObject(tileObjectController) < 2 && tileObjectController.GetName() != previousHighlight)
                {
                    validHighlight = true;
                }
            }
            previousHighlight = tileObjectController.GetName();

            currentTrial.result["study_highlight_" + numObjectsHit] = tileObjectController.GetName();
            string timeString = "time_highlight_" + numObjectsHit;

            if (studySetting != null)
            {
                studySetting.RecordHighlight(tileObjectController);
            }

            // Small delay
            yield return new WaitForSeconds(delay);

            highlight.gameObject.SetActive(true);
            tileObjectController.EnableStudyInteraction(this, studyLaserInteraction);

            DataManager.RecordTime(timeString);
        }

        public void HitTestTarget(StimulusMemory response)
        {
            string timeString = "time_hit_target";
            DataManager.RecordTime(timeString);

            currentTrial.result["reach_response"] = response.ToString();

            correctTrialResponse = response == reachObjectMemoryAnswer;
            currentTrial.result["correct_reach_response"] = correctTrialResponse;

            if (currentTrialType == TrialType.Baseline)
            {
                // End baseline trial at reach response
                endTrial = true;
            }
            else
            {
                // If an old stimuli, place it on the table
                if (response == StimulusMemory.Old)
                {
                    if (useControllerHints) { controllerHints.NextHint(); }

                    spawnLocationsManager.EnableTestInteraction(testObject);

                    testObject.EnableTestInteraction();
                    responseManager.ShowResponses(false);

                    insideConfirmButton.gameObject.SetActive(true);
                    outsideConfirmButton.gameObject.SetActive(true);

                    DataManager.RecordLocationForTrial(currentTrial, "test_placement_start", testObject);
                }
                // If a new stimuli, don't do anthing else
                else
                {
                    endTrial = true;
                }
            }
        }

        public void ConfirmLocation()
        {
            DataManager.RecordLocationForTrial(currentTrial, "test_placement_end", testObject);
            DataManager.RecordTime("time_confirm_location");

            Debug.Log("\tRemembered location: " + testObject.GetPosition().ToString());

            if (studySetting.HasStoredLocation(testObject))
            {
                Vector3 studyObjectLocation = studySetting.GetLocationForObject(testObject);
                Vector3 placementDiff = testObject.GetPosition() - studyObjectLocation;
                Debug.Log("\tPlacement error: " + placementDiff.ToString());
            }

            endTrial = true;
        }

        // Called from TimeManager when the timer sequence ends
        public void TimerEnd()
        {
            timedOutResponse = true;
            endTrial = true;
        }

        // Called to force-stop a trial, e.g. for motion sickness
        public void ForceEnd()
        {
            forceStop = true;
            endTrial = true;
        }

        // Coroutine for the trial behaviour
        IEnumerator TaskTrialSequence(Trial trial)
        {
            TrialType trialType = (TrialType)trial.settings.GetObject("trial_type");
            if (trialType == TrialType.Study && useStudyTimer)
            {
                timeManager.BeginStudyCountdown();
            }

            startingPoint.GetComponent<StartingPointController>().ToggleRenderer(false);
            if (trialType == TrialType.Study)
            {
                startingPoint.GetComponent<StartingPointController>().ToggleCollider(false);
            }

            if (useControllerHints) { controllerHints.NextHint(); }

            while (!endTrial) { yield return null; }

            Debug.Log("Response finished...");

            timeManager.StopCountdown();
            dataManager.FinishTrialPosition(trial);

            if (timedOutResponse)
            {
                // Timer ended before participant finished responding
                audioController.PlayAudioForTrial(false, startingPoint.gameObject.transform.position);
                DataManager.TrialTimedOut(trial);
            }
            else if (forceStop)
            {
                // Experimenter stopped the trial
                DataManager.TrialForceStopped(trial);
            }
            else
            {
                // Valid trial response
                // audioController.PlayAudioForTrial(correctTrialResponse, startingPoint.gameObject.transform.position);
                DataManager.TrialValidResponse(trial);
            }

            // Study data
            if (hitObjects.Count > 0)
            {
                DataManager.RecordTrialResultList(trial, "hit_objects", hitObjects);
                DataManager.RecordTrialResultList(trial, "hit_object_times", hitTimes);
            }
            if (studySetting != null)
            {
                DataManager.RecordHighlights(trial, studySetting.GetHighlightObjects());
            }

            // Test data
            if (pickupTimes.Count > 0)
            {
                DataManager.RecordHoldTimes(trial, pickupTimes, dropTimes);
            }
            if (placementTimes.Count > 0)
            {
                DataManager.RecordTrialResultList(trial, "test_placement_locations", testObjectPlacements);
                DataManager.RecordTrialResultList(trial, "test_placement_times", placementTimes);
            }

            // small delay to allow recording on a few extra frames into the response location
            yield return new WaitForSeconds(0.05f);

            Debug.Log("Ending trial...");
            // end current trial
            trial.End();
        }
    }
}
