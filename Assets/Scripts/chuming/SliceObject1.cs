using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Hanzzz.MeshSlicerFree
{
    public class SliceObject1 : MonoBehaviour
    {
        public Transform startSlicePoint;
        public Transform middleSlicePoint;
        public Transform endSlicePoint;
        public LayerMask sliceableLayer;
        public VelocityEstimator velocityEstimator;
        public Material croosSectionMaterial;
        public float cutForce = 200;

        public float cooldown = 1f;
        private float lastTriggerTime = -Mathf.Infinity;

        private MeshSlicer meshSlicer = new MeshSlicer();

        private (Vector3, Vector3, Vector3) Get3PointsOnPlane(Plane p)
        {
            Vector3 xAxis;
            if (0f != p.normal.x)
            {
                xAxis = new Vector3(-p.normal.y / p.normal.x, 1f, 0f);
            }
            else if (0f != p.normal.y)
            {
                xAxis = new Vector3(0f, -p.normal.z / p.normal.y, 1f);
            }
            else
            {
                xAxis = new Vector3(1f, 0f, -p.normal.x / p.normal.z);
            }
            Vector3 yAxis = Vector3.Cross(p.normal, xAxis);
            return (-p.distance * p.normal, -p.distance * p.normal + xAxis, -p.distance * p.normal + yAxis);
        }

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
                if (projectedSpeed > 0.6 && Time.time - lastTriggerTime >= cooldown)
                {
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

            (GameObject, GameObject) result = meshSlicer.Slice(target, Get3PointsOnPlane(new Plane(planeNormal, endSlicePoint.position)), croosSectionMaterial);
            CopyCustomComponents(result.Item1, target);
            SetupSlicedComponent(result.Item1, target);
            CopyCustomComponents(result.Item2, target);
            SetupSlicedComponent(result.Item2, target);
            // sliceTarget.SetActive(false);
            Destroy(target);
        }

        public void SetupSlicedComponent(GameObject sliceObject, GameObject oldObject)
        {
            Rigidbody rb = sliceObject.AddComponent<Rigidbody>();
            MeshCollider collider = sliceObject.AddComponent<MeshCollider>();
            XRGrabInteractable grab = sliceObject.AddComponent<XRGrabInteractable>();
            //sliceObject.AddComponent<DisableGrabbingHandModel>();
            //sliceObject.AddComponent<CookableItem>();
            collider.convex = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.AddExplosionForce(cutForce, sliceObject.transform.position, 1);
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grab.useDynamicAttach = true;

            sliceObject.layer = oldObject.layer;
            sliceObject.GetComponent<MushroomProperties>().isSliced = true;
        }

        public void CopyCustomComponents(GameObject sliceObject, GameObject oldObject)
        {
            foreach (var comp in oldObject.GetComponents<MonoBehaviour>())
            {
                Type t = comp.GetType();

                // 过滤掉不能复制的类型（比如 XRGrabInteractable）
                if (typeof(XRBaseInteractable).IsAssignableFrom(t))
                    continue;
                if (typeof(Collider).IsAssignableFrom(t))
                    continue;
                if (typeof(Rigidbody).IsAssignableFrom(t))
                    continue;

                // 新建组件
                var newComp = sliceObject.AddComponent(t);

                // 复制序列化字段
                var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var f in fields)
                {
                    if (f.IsDefined(typeof(SerializeField), true) || f.IsPublic)
                    {
                        f.SetValue(newComp, f.GetValue(comp));
                    }
                }
            }
        }
    }

}

