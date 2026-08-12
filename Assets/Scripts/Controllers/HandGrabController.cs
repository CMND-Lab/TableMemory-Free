using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UXF;

namespace TableMemory
{
    public class HandGrabController : MonoBehaviour
    {
        public SteamVR_Action_Boolean grabAction = null;
        public SteamVR_Behaviour_Pose pose = null;
        public TaskController taskController;

        private FixedJoint joint = null;

        private InteractableController currentInteractable = null;
        [SerializeField] List<InteractableController> contactInteractables = new List<InteractableController>();

        [SerializeField] bool resetObjectOnDrop = true;

        private void Awake()
        {
            joint = GetComponent<FixedJoint>();
        }

        private void Update()
        {
            // Pickup object
            if (grabAction.GetStateDown(pose.inputSource))
            {
                PickupObject();
            }

            // Drop object
            if (grabAction.GetStateUp(pose.inputSource))
            {
                DropObject();
            }
        }

        private void PickupObject()
        {
            currentInteractable = GetNearestInteractable();
            if (!currentInteractable)
                return;
            
            joint.connectedBody = currentInteractable.PickedUp(this);

            if (taskController != null && Session.instance.InTrial) {
                taskController.PickupObject();
            }
        }

        private void DropObject()
        {
            if (!currentInteractable) return;

            // Drop in area
            Rigidbody targetBody = currentInteractable.GetRB();
            targetBody.velocity = pose.GetVelocity();
            targetBody.angularVelocity = pose.GetAngularVelocity();

            // Hook
            currentInteractable.Dropped();
            if (currentInteractable.InDropArea())
            {
                Debug.Log("Dropping object into area...");
                currentInteractable.DropOntoArea(true);
                if (taskController != null && Session.instance.InTrial) {
                    taskController.DropObject();
                }
            }
            else if (resetObjectOnDrop) {
                Debug.Log("Resetting object...");
                currentInteractable.ResetPosition();
                if (taskController != null && Session.instance.InTrial) {
                    taskController.ResetObject();
                }
            }

            // Remove object from this this joint
            joint.connectedBody = null;
            currentInteractable = null;
        }

        public void Reset() {
            contactInteractables = new List<InteractableController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Interactable"))
            {
                contactInteractables.Add(other.gameObject.GetComponent<InteractableController>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Interactable"))
            {
                contactInteractables.Remove(other.gameObject.GetComponent<InteractableController>());
            }
        }

        private InteractableController GetNearestInteractable()
        {
            InteractableController nearest = null;
            float nearestDist = float.MaxValue;

            List<InteractableController> removeControllers = new List<InteractableController>();

            foreach (InteractableController controller in contactInteractables)
            {
                if (controller == null) {
                    removeControllers.Add(controller);
                    continue;
                }
                float dist = (gameObject.transform.position - controller.transform.position).sqrMagnitude;

                if (dist < nearestDist)
                {
                    nearest = controller;
                    nearestDist = dist;
                }
            }

            foreach (InteractableController c in removeControllers) {
                contactInteractables.Remove(c);
            }

            return nearest;
        }
    }
}

