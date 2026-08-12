using System.Collections.Generic;
using System.Windows.Forms;
using Unity;
using UnityEditor.UI;
using UnityEngine;
using UXF;
using Valve.VR.Extras;

namespace TableMemory
{
    public class TileController : MonoBehaviour
    {
        public TaskController taskController;

        [SerializeField] GameObject activeObject;
        private BoxCollider collider;

        private void Awake()
        {
            collider = GetComponent<BoxCollider>();
            collider.enabled = false;
            //EnableLocationTrigger();
        }

        public void SetObject(GameObject obj)
        {
            activeObject = obj;
        }

        public GameObject GetObject() { return activeObject; }
    }
}