using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostController : MonoBehaviour
{

    public GameObject controllerModel;
    public Renderer gloveRenderer;
    //public Renderer controllerBody = null;

    public Material transparentControllerMat;
    public Material opaqueControllerMat;
    public Material transparentGloveMat;
    public Material opaqueGloveMat;
    // Start is called before the first frame update
    void Start()
    {
        //MakeTransparent();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            MakeTransparent();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            MakeOpaque();
        }
    }

    public void MakeOpaque()
    {
        gloveRenderer.material = opaqueGloveMat;
        Renderer[] controllerRenderers = controllerModel.GetComponentsInChildren<Renderer>();
        foreach (var r in controllerRenderers)
        {
            r.material = opaqueControllerMat;
        }
    }

    public void MakeTransparent()
    {
        gloveRenderer.material = transparentGloveMat;

        Renderer[] controllerRenderers = controllerModel.GetComponentsInChildren<Renderer>();
        foreach (var r in controllerRenderers)
        {
            r.material = transparentControllerMat;
        }
    }
}
