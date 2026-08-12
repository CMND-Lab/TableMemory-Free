using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using UXFExamples;

public class StartingPointController : MonoBehaviour
{
    public Material responseOrbMat;
    public Material responseOrbMatLight;

    private Collider orbCollider;
    private MeshRenderer orbRenderer;

    private Collider OrbCollider()
    {
        if (orbCollider == null) { orbCollider = GetComponent<Collider>(); }
        return orbCollider;
    }

    private MeshRenderer OrbRenderer()
    {
        if (orbRenderer == null) { orbRenderer = GetComponent<MeshRenderer>(); }
        return orbRenderer;
    }

    void OnTriggerEnter(Collider other)
    {
        OrbRenderer().material = responseOrbMatLight;
        Debug.Log("Holding in Starting Orb");
    }

    void OnTriggerExit(Collider other)
    {
        OrbRenderer().material = responseOrbMat;
        Debug.Log("Exited the Starting Orb");
    }

    public void LightOn()
    {
        OrbRenderer().material = responseOrbMatLight;
    }
    public void LightOff()
    {
        OrbRenderer().material = responseOrbMat;
    }

    public void ToggleCollider(bool active)
    {
        OrbCollider().enabled = active;
    }

    public void ToggleRenderer(bool active)
    {
        OrbRenderer().enabled = active;
    }

    public void ResetState()
    {
        LightOff();
        ToggleCollider(true);
        ToggleRenderer(true);
    }

    public void Disappear()
    {
        ToggleCollider(false);
        ToggleRenderer(false);
    }
}

