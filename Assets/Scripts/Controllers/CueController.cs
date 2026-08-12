using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TableMemory
{
    public class CueController : MonoBehaviour
    {
        private TextMesh cueText;

        //public Collider collider;
        
        private void Awake()
        {
            cueText = transform.GetChild(0).gameObject.GetComponent<TextMesh>();
            //collider = GetComponent<Collider>();
            //collider.enabled = false;
        }

        public void ResetToNormal()
        {
            // cueText.text = string.Empty;
            //collider.enabled = false;
        }

        public void EnableCue()
        {
            cueText.text = "+";
            //collider.enabled = true;
        }

        public void DisableCue()
        {
            cueText.text = string.Empty;
            // collider.enabled = false;
        }
    }
}

