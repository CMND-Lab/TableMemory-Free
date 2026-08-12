using System.Collections.Generic;
using System.Windows.Forms;
using Unity;
using UnityEditor.UI;
using UnityEngine;
using UXF;

namespace TableMemory {
    interface TrialSetting {
        public TrialType GetTrialType();
        public List<ObjectController> GetStimuli();
        public ObjectController GetStimulus();
    }

    public class StudyTrialSetting : TrialSetting
    {
        private List<ObjectController> stimuli = null;
        private int numHighlights;

        private Dictionary<string, Vector3> studyLocations;
        private List<string> highlightedObjects;
        
        public StudyTrialSetting(List<ObjectController> objs, int highlights)
        {
            this.stimuli = objs;
            this.numHighlights = highlights;

            studyLocations = new Dictionary<string, Vector3>();
            highlightedObjects = new List<string>();
        }

        public void StoreObjectLocations(List<ObjectController> objectControllers)
        {
            foreach (ObjectController objController in objectControllers)
            {
                studyLocations.Add(objController.GetName(), objController.GetPosition());
            }
        }

        public bool HasStoredLocation(ObjectController controller)
        {
            return studyLocations.ContainsKey(controller.GetName());
        }

        public Vector3 GetLocationForObject(ObjectController controller)
        {
            return studyLocations[controller.GetName()];
        }

        public List<ObjectController> GetStimuli() { return stimuli; }
        public List<GameObject> GetStimuliGameObjects()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            foreach (ObjectController obj in stimuli)
            {
                gameObjects.Add(obj.gameObject);
            }
            return gameObjects;
        }
        public ObjectController GetStimulus() { return null; }
        public TrialType GetTrialType() { return TrialType.Study; }
        public int GetNumHighlights() { return numHighlights; }

        public void RecordHighlight(ObjectController controller)
        {
            highlightedObjects.Add(controller.GetName());
        }
        
        public List<string> GetHighlightObjects()
        {
            return highlightedObjects;
        }

        public int GetNumHighlightsForObject(ObjectController controller)
        {
            int appearances = 0;
            foreach (string s in highlightedObjects)
            {
                if (s == controller.GetName()) { appearances++; }
            }
            return appearances;
        }
    }

    public class TestTrialSetting : TrialSetting 
    {
        private ObjectController stimulus;
        private StimulusMemory memoryStatus;

        public TestTrialSetting(ObjectController obj, StimulusMemory memory)
        {
            this.stimulus = obj;
            this.memoryStatus = memory;
        }
        public ObjectController GetStimulus() { return stimulus; }
        public List<ObjectController> GetStimuli() { return new List<ObjectController>(); }
        public TrialType GetTrialType() { return TrialType.Test; }

        public StimulusMemory GetMemoryStatus()
        {
            return memoryStatus;
        }
    }
}