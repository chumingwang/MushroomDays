using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BasketSettings : MonoBehaviour
{
    Rigidbody rb;
    XRGrabInteractable grab;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 5f)
            rb.velocity = rb.velocity.normalized * 5f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mushroom"))
            other.GetComponent<Rigidbody>().drag = 5f;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mushroom"))
            other.GetComponent<Rigidbody>().drag = 0.1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
