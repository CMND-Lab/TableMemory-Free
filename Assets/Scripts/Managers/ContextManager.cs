using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using Valve.VR.Extras;

namespace TableMemory
{
    public class ContextManager : MonoBehaviour
    {
        public Session session;
        public TeleportAreaController insideTeleportArea;
        public GameObject insideExperimentDisplay;

        public TeleportAreaController outsideTeleportArea;
        public GameObject outsideExperimentDisplay;

        public DoorController door;

        public GameObject experiment;
        public CanvasController canvasController;

        private SteamVR_LaserPointer laserPointer;

        [SerializeField] Transform insideCanvasLocation;
        [SerializeField] Transform outsideCanvasLocation;

        private LocationContext currentContext = LocationContext.Inside;

        private float contextSwitchTime = 5.0f;
        public ControllerHints controllerHints;
        private bool useHints = false;


        private bool finishedContextSwitch = false;
        public bool FinishedContextSwitch()
        {
            return finishedContextSwitch;
        }

        private void Awake()
        {
            laserPointer = GameObject.FindGameObjectWithTag("Laser").GetComponent<SteamVR_LaserPointer>();

            insideTeleportArea.Activate(false);
            outsideTeleportArea.Activate(false);
            // SetLocation(LocationContext.Inside);

            //outsideExperimentDisplay.SetActive(false);
        }

        public void Initialise()
        {
            door.EnableInteraction(false);
        }

        public void EnableHints()
        {
            useHints = true;
            controllerHints.ShowSwitchHints();
        }

        public void ChangeLocation(Trial trial=null)
        {
            finishedContextSwitch = false;

            laserPointer.holder.SetActive(true);
            laserPointer.pointer.SetActive(true);

            Debug.Log("Switching locations...");
            switch (currentContext)
            {
                case LocationContext.Inside:
                    StartCoroutine(SwitchToLocation(LocationContext.Outside, trial)); break;
                case LocationContext.Outside:
                    StartCoroutine(SwitchToLocation(LocationContext.Inside, trial)); break;
            }
        }

        private IEnumerator SwitchToLocation(LocationContext newContext, Trial trial=null)
        {
            if (currentContext == null || currentContext != newContext)
            {
                if (trial != null)
                {
                    // Show instructions
                    canvasController.SetCanvasState(CanvasState.ContextChange);
                    canvasController.ShowInstructions(true);
                    canvasController.SetInstruction("Please go to the " + newContext.ToString().ToLower() + " table.");
                }
                // Set new context
                currentContext = newContext;

                // Show table display at new context
                insideExperimentDisplay.SetActive(newContext == LocationContext.Inside);
                outsideExperimentDisplay.SetActive(newContext == LocationContext.Outside);

                // Enable door open/closing
                door.EnableInteraction();

                float startContextSwitchTime = Time.time;

                yield return new WaitUntil(door.DoorIsOpen);

                // Show teleport area at new context
                insideTeleportArea.Activate(newContext == LocationContext.Inside);
                outsideTeleportArea.Activate(newContext == LocationContext.Outside);

                if (useHints) controllerHints.NextHint();

                door.UseOpeningCollider(false);

                TeleportAreaController activeTeleporter = newContext == LocationContext.Inside ? insideTeleportArea : outsideTeleportArea;
                yield return new WaitUntil(activeTeleporter.HasBeenTeleportedTo);

                // User has teleported
                if (useHints) controllerHints.NextHint();

                // Turn teleport areas off
                insideTeleportArea.Activate(false);
                outsideTeleportArea.Activate(false);

                // Switch canvas position
                ChangeCanvas(currentContext);

                // Make sure user has closed door
                canvasController.SetInstruction("Please close the door.");

                door.UseOpeningCollider(true);
                yield return new WaitUntil(door.DoorIsClosed);

                if (useHints) controllerHints.NextHint();

                // Disable door interaction
                door.OpenDoor(false);
                door.EnableInteraction(false);

                float endContextSwitchTime = Time.time;

                Debug.Log("Switch time:\n\tStart: " + startContextSwitchTime.ToString() + "\n\tEnd: " + endContextSwitchTime.ToString() + "\n\tTotal: " + (endContextSwitchTime - startContextSwitchTime).ToString());
                if (trial != null) { 
                    trial.result["location_switch_time"] = contextSwitchTime.ToString(); 
                    contextSwitchTime = endContextSwitchTime - startContextSwitchTime; 
                }
                
                useHints = false;
            }
            finishedContextSwitch = true;
        }

        public float GetContextSwitchTime()
        {
            return contextSwitchTime;
        }

        public void NoSwitchDelay(ExperimentManager experimentManager, Trial trial)
        {
            trial.result["no_switch_timedelay"] = contextSwitchTime.ToString();
            StartCoroutine(DelayForSwitchTime(experimentManager));
        }

        IEnumerator DelayForSwitchTime(ExperimentManager experimentManager)
        {
            experimentManager.DisableExperiment();

            canvasController.SetCanvasState(CanvasState.ContextChange);
            canvasController.SetInstruction("Loading memory test.....");
            canvasController.ShowInstructions(true);

            Debug.Log(contextSwitchTime.ToString());
            yield return new WaitForSeconds(contextSwitchTime);

            canvasController.ShowInstructions(false);
            experimentManager.EnableExperiment();
        }

        private void ChangeExperimentLocation(LocationContext newContext)
        {
            Vector3 newLocation = new Vector3();

            // Move x and z co-ordinates to new location
            newLocation.x = newContext == LocationContext.Inside ? insideExperimentDisplay.transform.position.x : outsideExperimentDisplay.transform.position.x;
            newLocation.y = experiment.transform.position.y;
            newLocation.z = newContext == LocationContext.Inside ? insideExperimentDisplay.transform.position.z : outsideExperimentDisplay.transform.position.z;
        
            experiment.transform.position = newLocation;
        }

        private void ChangeCanvas(LocationContext visibleContext)
        {
            canvasController.gameObject.transform.position = visibleContext == LocationContext.Inside ? insideCanvasLocation.position : outsideCanvasLocation.position;
        }

        public LocationContext CurrentContext()
        {
            return currentContext;
        }
    }

    public enum LocationContext
    {
        Inside,
        Outside
    }
}


