using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using Valve.VR.Extras;

namespace TableMemory
{
    public class TeleportAreaController : MonoBehaviour
    {
        private SteamVR_LaserPointer laserPointer;
        [SerializeField] Transform teleportOrigin;

        [SerializeField] Material onMat;
        [SerializeField] Material offMat;

        private Collider collider;
        private Renderer renderer;

        [SerializeField] List<GameObject> shiftObjects;

        private bool hasBeenTeleportedTo;

        private void Awake()
        {
            laserPointer = GameObject.FindGameObjectWithTag("Laser").GetComponent<SteamVR_LaserPointer>();
            collider = GetComponent<Collider>();
            renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            Ray laserRay = new Ray(laserPointer.transform.position, laserPointer.transform.forward);
            RaycastHit hit;

            if (collider.Raycast(laserRay, out hit, 30.0f))
            {
                renderer.material = onMat;
            }
            else
            {
                renderer.material = offMat;
            }
        }

        private void OnEnable()
        {
            laserPointer.PointerClick += LaserPointer;
        }

        private void OnDisable()
        {
            laserPointer.PointerClick -= LaserPointer;
        }

        private void LaserPointer(object sender, PointerEventArgs e)
        {
            if (e.target.gameObject == this.gameObject)
            {
                if (teleportOrigin == null) { teleportOrigin = this.transform; }

                float adjust_x = teleportOrigin.position.x;
                float adjust_z = teleportOrigin.position.z;

                foreach (GameObject o in shiftObjects)
                {
                    Vector3 newPosition = new Vector3(
                        o.transform.position.x - adjust_x, // Teleport on x-axis
                        o.transform.position.y, // Keep position on y-axis
                        o.transform.position.z - adjust_z // Teleport on z-axis
                    );
                    o.transform.position = newPosition;
                }

                
                hasBeenTeleportedTo = true;
            }
        }

        public void Activate(bool enable)
        {
            hasBeenTeleportedTo = false;
            gameObject.SetActive(enable);
        }

        public bool HasBeenTeleportedTo()
        {
            return hasBeenTeleportedTo;
        }
    }
}

