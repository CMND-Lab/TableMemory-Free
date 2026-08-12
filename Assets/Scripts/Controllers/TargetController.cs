using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TableMemory;
using UnityEngine;
using UXF;

public class TargetController : MonoBehaviour
{
    public TaskController taskController;
    public StimulusMemory responseValue;

    [SerializeField] GameObject label;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter target: " + responseValue.ToString());
        if (other.CompareTag("Controller"))
        {
            taskController.HitTestTarget(responseValue);
        }
    }

    public void ShowLabel(bool show)
    {
        label.SetActive(show);
    }

    public void ToggleVisibility(bool show)
    {
        gameObject.SetActive(show);
    }

    public void SetMaterial(Material mat)
    {
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    public void EnableCollider(bool enable)
    {
        gameObject.GetComponent<Collider>().enabled = enable;
    }
}