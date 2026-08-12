using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

namespace TableMemory
{
    public class ConfirmController : MonoBehaviour
    {
        private SteamVR_LaserPointer laserPointer;
        [SerializeField] TaskController taskController;

        private void OnEnable()
        {
            laserPointer = GameObject.FindGameObjectWithTag("Laser").GetComponent<SteamVR_LaserPointer>();
            laserPointer.PointerClick += LaserPointer;
            // laserPointer.PointerIn += LaserHit;
            // laserPointer.PointerOut += LaserExit;
        }

        private void OnDisable()
        {
            laserPointer.PointerClick -= LaserPointer;
        }

        private void LaserPointer(object sender, PointerEventArgs e)
        {
            if (e.target.gameObject == this.gameObject)
            {
                taskController.ConfirmLocation();
            }
        }

        public void EnableClick(bool enable)
        {
            gameObject.GetComponent<BoxCollider>().enabled = enable;
        }
    }
}

