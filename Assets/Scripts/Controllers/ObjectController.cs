using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using Valve.VR.Extras;

namespace TableMemory
{
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectController : MonoBehaviour
    {
        
        private InteractableController interactable;
        [SerializeField] BoxCollider collider;

        private SteamVR_LaserPointer laserPointer;
        [SerializeField] bool useLaser = false;

        private List<Renderer> renderers;
        private TaskController taskController;

        [SerializeField] Color outlineColour = Color.white;

        private void Awake()
        {
            laserPointer = GameObject.FindGameObjectWithTag("Laser").GetComponent<SteamVR_LaserPointer>();
            renderers = new List<Renderer>();
            renderers.AddRange(GetComponentsInChildren<Renderer>());

            // Object controller will always be at the root of the object, so it allocates these dependencies
            // InteractableController can be in a child object, so won't always have implicit access to collider and rb
            interactable = GetComponentInChildren<InteractableController>(true);
            collider = GetComponentInChildren<BoxCollider>(true);
            
            Rigidbody rb = GetComponentInChildren<Rigidbody>(true);
            rb.isKinematic = true;

            if (interactable != null && rb != null) 
            {
                interactable.SetRB(rb);
                interactable.SetRoot(this.gameObject);
            }

            // Disable interaction by default
            interactable.gameObject.SetActive(false);
        }

        public void SetLayer(int layer) 
        {
            var children = gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = layer;
            }
        }

        public void EnableLowGravity() 
        {
            laserPointer.PointerClick += this.PointerClick;
            interactable.EnableLowGravity();
            useLaser = true;
            interactable.CreateOutliners(renderers, outlineColour);
        }

        public void CenterPosition(Vector3 position)
        {
            Vector3 diffFromCentre = interactable.gameObject.transform.position - position;
            gameObject.transform.position = gameObject.transform.position - diffFromCentre;

            interactable.SetRoot(this.gameObject);
        }

        public void EnableStudyInteraction(TaskController taskController, bool useLaser)
        {
            this.taskController = taskController;

            if (useLaser)
            {
                EnableLaser();
            }
            else
            {
                interactable.EnableStudyInteraction(this);
            }
        }

        public void EnableTestInteraction()
        {
            interactable.EnableTestInteraction();
            interactable.CreateOutliners(renderers, outlineColour);
            interactable.SetRoot(this.gameObject);
        }

        public void HitStudyObject()
        {
            taskController.HitStudyObject(this);
        }

        public void ExitStudyObject()
        {
            taskController.ExitStudyObject(this);
        }

        public void EnableLaser()
        {
            if (laserPointer != null)
            {
                Debug.Log("Enabled laser on " + GetName());
                laserPointer.PointerClick += this.PointerClick;

                interactable.EnableLaser();
            }
        }

        public void DisableLaser()
        {
            if (laserPointer != null)
            {
                laserPointer.PointerClick -= this.PointerClick;
                interactable.gameObject.SetActive(false);
            }
        }

        private void PointerClick(object sender, PointerEventArgs e)
        {
            if (e.target.gameObject == this.gameObject) {
                Debug.Log("Laser hit!");
                if (useLaser) {
                    Debug.Log("Go to laser...");
                    interactable.GoToLaser(laserPointer);
                }

                if (Session.instance.InTrial)
                {
                    taskController.HitStudyObject(this);
                }
            }
        }

        public Vector3 GetSize()
        {
            Vector3 sizeVec;
            bool wasDisabled = false;

            if (!interactable.enabled) 
            { 
                interactable.gameObject.SetActive(true); 
                wasDisabled = true; 
            }

            sizeVec = collider.bounds.size;

            if (wasDisabled) 
            { 
                interactable.gameObject.SetActive(false);
            }

            return sizeVec;
        }

        public string GetName()
        {
            return gameObject.name.ToLower().Replace(" ", "_");
        }

        public Vector3 GetPosition()
        {
            return interactable.gameObject.transform.position;
        }
        
        public Vector3 GetRotation()
        {
            return interactable.gameObject.transform.rotation.eulerAngles;
        }
    }
}