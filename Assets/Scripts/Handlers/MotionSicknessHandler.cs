using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace TableMemory
{
    public class MotionSicknessHandler : MonoBehaviour
    {
        public Session session;

        public TaskController taskController;

        public CanvasController canvasController;

        private void Update()
        {
            /*
             * Press BACKSPACE if a block needs to be stopped 
             */
            if (Input.GetKeyDown(KeyCode.Backspace) && (session.currentBlockNum != 1 || session.GetBlock(1).lastTrial == session.CurrentTrial))
            {
                // Debug.Log("CURRENT BLOCK NUMBER" + session.currentBlockNum);
                Debug.Log("Motion Sickness Event has been called");
                

                // End current trial if one is running
                if (session.CurrentTrial.status == TrialStatus.InProgress) {
                  taskController.ForceEnd();
                }

                // Set canvas state if not at end of block
                // If at end of block the canvas will be set anyway
                if (session.CurrentTrial != session.CurrentBlock.lastTrial) {
                    canvasController.SetCanvasState(CanvasState.Pause);

                    canvasController.SetInstruction(
                      "Experiment paused.<br>" +
                      "Please remove the headset and take a break.<br><br>" +
                      "Recalibrate before you continue the experiment."
                    );
                }
            }
        }
    }
}

