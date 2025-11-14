using EzySlice;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class sliceObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public LayerMask sliceableLayer;
    public VelocityEstimator velocityEstimator;
    public Material croosSectionMaterial;
    public float cutForce = 200;

    public float cooldown = 1f;
    private float lastTriggerTime = -Mathf.Infinity;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if (hasHit)
        {
            Vector3 velocity = velocityEstimator.GetVelocityEstimate();
            Vector3 dir = new Vector3(transform.up.x, -transform.up.y, transform.up.z).normalized;
            float projectedSpeed = Vector3.Dot(velocity, dir);
            if (projectedSpeed > 0.6 && Time.time - lastTriggerTime >= cooldown) {
                lastTriggerTime = Time.time;
                GameObject target = hit.transform.gameObject;
                Slice(target);
            }
            
        }
    }
    public void Slice(GameObject target)
    {
        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();


        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, croosSectionMaterial);
            upperHull.layer = target.layer;
            SetupSlicedComponent(upperHull);

            GameObject loverHull = hull.CreateLowerHull(target, croosSectionMaterial);
            loverHull.layer = target.layer;
            SetupSlicedComponent(loverHull);
            Destroy(target);
        }
    }

    public void SetupSlicedComponent(GameObject sliceObject)
    {
        Rigidbody rb = sliceObject.AddComponent<Rigidbody>();
        MeshCollider collider = sliceObject.AddComponent<MeshCollider>();
        XRGrabInteractable grab = sliceObject.AddComponent<XRGrabInteractable>();
        sliceObject.AddComponent<DisableGrabbingHandModel>();
        sliceObject.AddComponent<CookableItem>();
        collider.convex = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddExplosionForce(cutForce, sliceObject.transform.position, 1);
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grab.useDynamicAttach = true;
    }
}

