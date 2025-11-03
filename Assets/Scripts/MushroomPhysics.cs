using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MushroomPhysics : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // When player grabs, disable physics so hand controls position
        rb.useGravity = false;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // When player releases, enable gravity again
        rb.useGravity = true;
        //grabInteractable.selectEntered.RemoveListener(OnGrab);
        //grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}
