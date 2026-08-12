using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Linq;
using UXF;

public class ExperimentLocation : MonoBehaviour
{
    public GameObject experiment;
    private bool adjusted = false;
    public Vector3 headPosition;
    private List<XRNodeState> nodeStates = new List<XRNodeState>();
    public InputDevice device;

    [SerializeField] GameObject[] adjustLocations;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AdjustExperimentHeight();
        }
    }

    public void RecordObjectPositions(Trial trial) {

        bool experimentIsActive = experiment.activeSelf;
        experiment.SetActive(true);
        RecordBounds[] position_recorders = FindObjectsOfType<RecordBounds>(true);

        foreach (RecordBounds recorder in position_recorders)
        {
            bool recorderIsActive = recorder.gameObject.activeSelf;
            recorder.gameObject.SetActive(true);


            recorder.Record(trial);

            recorder.gameObject.SetActive(recorderIsActive);
        }

        experiment.SetActive(experimentIsActive);
    }

    IEnumerator RecordProcess()
    {
        yield return new WaitForSeconds(0f);
    }

    public float AdjustExperimentHeight()
    {
        InputTracking.GetNodeStates(nodeStates);
        var headState = nodeStates.FirstOrDefault(node => node.nodeType == XRNode.Head);
        headState.TryGetPosition(out headPosition);

        string format = "0.####";

        foreach (GameObject adjust in adjustLocations)
        {
            Transform object_pos = adjust.transform;
            Debug.Log("Current position for " + adjust.name + ": " + object_pos.position.ToString("F4"));

            object_pos.position = new Vector3(object_pos.position.x, headPosition.y, object_pos.position.z);
            Debug.Log("New position for " + adjust.name + ": " + object_pos.position.ToString("F4"));
        }

        Session.instance.participantDetails["height"] = headPosition.y;
        return headPosition.y;
    }
}