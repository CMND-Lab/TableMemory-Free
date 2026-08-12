using System;
using UnityEngine;
using UXF;

namespace TableMemory
{
    public class ResponseManager : MonoBehaviour
    {
        public TargetController oldResponse;
        public TargetController newResponse;
        public GameObject fakeOld;
        public GameObject fakeNew;
        [SerializeField] Material highlightMat;
        [SerializeField] Material defaultMat;

        public void Initialise()
        {
            int counterbalance = Convert.ToInt32(Session.instance.participantDetails["counterbalance"]);
            if (counterbalance == 0)
            {
                SwapResponses();
            }

            fakeOld.transform.position = oldResponse.transform.position;
            fakeNew.transform.position = newResponse.transform.position;
        }

        public void SwapResponses()
        {
            // Swap response orbs
            Vector3 tempPos = newResponse.transform.position;

            newResponse.transform.position = oldResponse.transform.position;
            oldResponse.transform.position = tempPos;
        }

        public void ShowResponses(bool show)
        {
            oldResponse.ToggleVisibility(show);
            newResponse.ToggleVisibility(show);
        }

        public void Reset()
        {
            oldResponse.SetMaterial(defaultMat);
            newResponse.SetMaterial(defaultMat);

            oldResponse.EnableCollider(false);
            newResponse.EnableCollider(false);
        }

        public void EnableResponse(StimulusMemory response)
        {
            gameObject.SetActive(true);
            switch (response) {
                case StimulusMemory.New:
                    newResponse.ToggleVisibility(true); break;
                case StimulusMemory.Old:
                    oldResponse.ToggleVisibility(true); break;
            }
        }

        public void EnableColliders(bool enable)
        {
            oldResponse.EnableCollider(enable);
            newResponse.EnableCollider(enable);
        }

        public void BaselineResponse(StimulusMemory response)
        {
            newResponse.SetMaterial(response == StimulusMemory.New ? highlightMat : defaultMat);
            newResponse.EnableCollider(response == StimulusMemory.New);
            newResponse.ToggleVisibility(response == StimulusMemory.New);

            oldResponse.SetMaterial(response == StimulusMemory.Old ? highlightMat : defaultMat);
            oldResponse.EnableCollider(response == StimulusMemory.Old);
            oldResponse.ToggleVisibility(response == StimulusMemory.Old);
        }

        public void HighlightResponse(StimulusMemory response)
        {
            gameObject.SetActive(true);
            switch (response)
            {
                case StimulusMemory.New:
                    newResponse.SetMaterial(highlightMat); break;
                case StimulusMemory.Old:
                    oldResponse.SetMaterial(highlightMat); break;
            }
        }

        public void ShowResponseLabels(bool show)
        {
            newResponse.ShowLabel(show);
            oldResponse.ShowLabel(show);
        }

        public void RecordPositions(Trial trial)
        {
            trial.result["left_response"] = oldResponse.transform.position.x < 0 ? oldResponse.name : newResponse.name;
            trial.result["right_response"] = oldResponse.transform.position.x > 0 ? oldResponse.name : newResponse.name;
        }
    }
}


