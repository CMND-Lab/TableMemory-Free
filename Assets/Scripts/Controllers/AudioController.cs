using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TableMemory
{
    public class AudioController : MonoBehaviour
    {
        public AudioClip audioIncorrect;
        public AudioClip audioCorrect;

        public void PlayAudioForTrial(bool correctResponse, Vector3 playAtLocation)
        {
            if (correctResponse)
            {
                AudioSource.PlayClipAtPoint(audioCorrect, playAtLocation, 0.1f);
            }
            else
            {
                AudioSource.PlayClipAtPoint(audioIncorrect, playAtLocation, 0.1f);
            }
        }
        
    }
}



