using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using UXFExamples;

namespace TableMemory
{
    public class StartingPointTrialController : MonoBehaviour
    {
        public GameObject experimentManager;
        public TaskController taskController;
        public Session session;
        private StartingStateVR state = StartingStateVR.Waiting;
        private Coroutine cueCoroutine;
    
        public GameObject cue;

        private float holdTime;

        private void Awake()
        {
            cue.SetActive(false);
        }

        public void SetHoldTime(float time)
        {
            holdTime = time;
        }

        IEnumerator CueSequence()
        {
            state = StartingStateVR.GetReady;
            yield return new WaitForSeconds(0.25f);
            cue.SetActive(true);
            yield return new WaitForSeconds(holdTime);
            cue.SetActive(false);
            // yield return new WaitForSeconds(0.1f);
            state = StartingStateVR.Go;
            session.BeginNextTrial();
        }
    
        public void ResetStartOrb()
        {
            state = StartingStateVR.Waiting;
            gameObject.GetComponent<StartingPointController>().ResetState();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Controller"))
            {
                switch (state)
                {
                    case StartingStateVR.Waiting:
                        cueCoroutine = StartCoroutine(CueSequence());
                        taskController.EnableLaser(false);
                        break;
                }
            }
        }
    
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Controller"))
            {
                gameObject.GetComponent<SphereCollider>().enabled = false;

                switch (state)
                {
                    case StartingStateVR.Go:
                        taskController.ExitStartOrb();

                        break;

                    default:
                        StopCoroutine(cueCoroutine);
                        cue.SetActive(false);
                        ResetStartOrb();
                        break;
                }
            }
        }
    }
    
    public enum StartingStateVR
    {
        Waiting, GetReady, Go
    }
}

