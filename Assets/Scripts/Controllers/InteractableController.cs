using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UXF;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;

namespace TableMemory
{
    [RequireComponent(typeof(BoxCollider))]
    public class InteractableController : MonoBehaviour
    {
        private Rigidbody rb;
        private GameObject root;

        [HideInInspector]
        public HandGrabController activeHand = null;

        [SerializeField] bool freezeWithoutGrab = true;

        private Vector3 rootPosition;
        private Quaternion rootRotation;

        private GameObject dropArea;
        private List<Outline> outliners;

        private InteractionState interactionState;
        private ObjectController objectController;

        private void Awake()
        {
            outliners = new List<Outline>();
        }

        public bool InDropArea()
        {
            return dropArea != null;
        }

        public void EnableLowGravity() {
            EnableLaser();
            EnableGrab();
            rb.useGravity = false;
            freezeWithoutGrab = false;
            gameObject.GetComponent<Collider>().isTrigger = false;        
            rb.isKinematic = false;
        }

        public void EnableLaser()
        {
            gameObject.SetActive(true);
            gameObject.tag = "Untagged";
        }

        public void EnableStudyInteraction(ObjectController controller)
        {
            objectController = controller;
            interactionState = InteractionState.Study;
            gameObject.GetComponent<Collider>().isTrigger = true;
            gameObject.SetActive(true);
            gameObject.tag = "Untagged";
        }

        public void EnableTestInteraction()
        {
            interactionState = InteractionState.Test;
            EnableGrab();
        }

        public void EnableGrab()
        {
            gameObject.GetComponent<Collider>().isTrigger = true;
            gameObject.SetActive(true);
            gameObject.tag = "Interactable";
        }

        public void SetRB(Rigidbody rb)
        {
            this.rb = rb;
            if (freezeWithoutGrab) rb.isKinematic = true;
        }

        public void SetRoot(GameObject root)
        {
            this.root = root;
            rootPosition = root.transform.position;
            rootRotation = root.transform.rotation;
        }

        public Rigidbody GetRB()
        {
            return rb;
        }

        public void GoToLaser(SteamVR_LaserPointer laserPointer)
        {
            Vector3 velDiff = laserPointer.gameObject.transform.position - this.gameObject.transform.position; 
            velDiff = velDiff.normalized;

            rb.velocity = velDiff;
        }

        public void Dropped()
        {
            if (freezeWithoutGrab) rb.isKinematic = true;
            activeHand = null;
        }

        public Rigidbody PickedUp(HandGrabController handGrabController)
        {
            //gameObject.transform.position = handGrabController.gameObject.transform.position;
            rb.isKinematic = false;
            activeHand = handGrabController;

            return GetRB();
        }

        public void DropOntoArea(bool resetOrientation = false)
        {
            root.transform.position = new Vector3(root.transform.position.x, dropArea.transform.position.y, root.transform.position.z);
            if (resetOrientation) root.transform.rotation = rootRotation;
        }

        public void ResetPosition()
        {
            root.transform.position = rootPosition;
            root.transform.rotation = rootRotation;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("DropArea"))
            {
                Debug.Log("Enter droparea");
                dropArea = other.gameObject;
            }
            else if (other.gameObject.CompareTag("Controller"))
            {
                if (interactionState == InteractionState.Study && objectController != null)
                {
                    objectController.HitStudyObject();
                }
                else if (interactionState == InteractionState.Test)
                {
                    ToggleOutline(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("DropArea"))
            {
                Debug.Log("Exit droparea");
                dropArea = null;
            }
            else if (other.gameObject.CompareTag("Controller"))
            {
                if (interactionState == InteractionState.Study && objectController != null)
                {
                    objectController.ExitStudyObject();
                }
                else if (interactionState == InteractionState.Test)
                {
                    ToggleOutline(false);
                }
            }
        }
        
        private void ToggleOutline(bool enable)
        {
            foreach (Outline o in outliners)
            {
                o.enabled = enable;
            }
        }

        public void CreateOutliners(List<Renderer> objectRenderers, UnityEngine.Color outlineColour)
        {
            foreach (Renderer r in objectRenderers)
            {
                Outline outline = r.gameObject.AddComponent<Outline>();
                outline.OutlineColor = outlineColour;
                outline.enabled = false;

                outliners.Add(outline);
            }
        }

        public void SetInteraction(InteractionState interaction)
        {
            interactionState = interaction;
        }

    }
    public enum InteractionState
    {
        Test,
        Study
    }
}

