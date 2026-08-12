using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TableMemory
{
    public class StimulusCollectionManager : MonoBehaviour
    {
        [SerializeField] List<ObjectController> allStimuli;
        private List<ObjectController> unpickedStimuli;
        
        [InspectorButton("EnableLowGravity")]
        public bool LowGravity;

        private void EnableLowGravity()
        {
            string s = "";
            foreach (ObjectController oc in allStimuli)
            {
                oc.EnableLowGravity();
                s += oc.gameObject.name + "\n";
            }
            Debug.Log(s);
        }

        private void Awake()
        {
            allStimuli = GetComponentsInChildren<ObjectController>(false).ToList();
            Debug.Log("Found " + allStimuli.Count + " objects");
            unpickedStimuli = allStimuli.ToList();
        }

        private List<ObjectController> _PickRandomObjects(int numberOfObjects, List<ObjectController> objectList)
        {
            int numberOfObjectsToPick = Mathf.Min(numberOfObjects, objectList.Count);
            Debug.Log("Selecting " + numberOfObjectsToPick + " objects...");

            // Copy list of all objects, so we don't alter anything
            List<ObjectController> returnObjects = new List<ObjectController>();

            for (int i = 0; i < numberOfObjectsToPick; i++)
            {
                // Pick a random object from the list
                int randomObjectIndex = Random.Range(0, objectList.Count);
                ObjectController randomObject = objectList[randomObjectIndex];
                Debug.Log("Picked: " + randomObject.GetName());

                // Store the random object to be returned
                returnObjects.Add(randomObject);
                // Remove the random object so it can't be picked twice
                objectList.Remove(randomObject);
            }

            return returnObjects;
        }

        public List<ObjectController> GetRandomStimuli(int numberOfObjects)
        {
            List<ObjectController> copyOfAllObjects = new List<ObjectController>(allStimuli);
            return _PickRandomObjects(numberOfObjects, copyOfAllObjects);
        }

        public List<ObjectController> GetRandomUnpickedStimuli(int numberOfObjects)
        {
            return _PickRandomObjects(numberOfObjects, unpickedStimuli);
        }

        public int NumUnpickedStimuli()
        {
            return unpickedStimuli.Count;
        }
    }
}


