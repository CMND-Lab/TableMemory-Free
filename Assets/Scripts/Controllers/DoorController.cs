using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

namespace TableMemory
{
    public class DoorController : MonoBehaviour
    {
        Animator animator;
        private bool doorIsOpen = false;

        private SteamVR_LaserPointer laserPointer;
        private Collider doorOpeningCollider;

        [SerializeField] GameObject doorModel;
        private Collider doorModelCollider;

        void Awake()
        {
            laserPointer = GameObject.FindGameObjectWithTag("Laser").GetComponent<SteamVR_LaserPointer>();
            animator = GetComponent<Animator>();
            doorOpeningCollider = GetComponent<Collider>();
            doorModelCollider = doorModel.GetComponent<Collider>();
        }

        public void OpenDoor(bool open = true)
        {
            if (open == doorIsOpen) return;

            if (open)
            {
                animator.Play("DoorOpen", 0, 0.0f);
            }
            else
            {
                animator.Play("DoorClose", 0, 0.0f);
            }
            doorIsOpen = open;
        }

        public void ToggleDoor()
        {
            OpenDoor(!doorIsOpen);
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
            if (e.target.gameObject == this.gameObject || e.target.gameObject == doorModel)
            {
                ToggleDoor();
            }
        }

        public void EnableInteraction(bool enable = true)
        {
            doorOpeningCollider.enabled = enable;
            doorModelCollider.enabled = enable;
        }

        public void UseOpeningCollider(bool enable = true)
        {
            doorOpeningCollider.enabled = enable;
        }

        public bool DoorIsOpen()
        {
            return doorIsOpen;
        }

        public bool DoorIsClosed()
        {
            return !doorIsOpen;
        }
    }
}

