using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TableMemory
{
    public class QuitExperiment : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("THE EXPERIMENT HAS BEEN STOPPED");
#if UNITY_EDITOR_WIN
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}