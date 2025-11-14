// MushroomTrackerUIPositioner.cs
// Positions the UI in front of the camera/player for VR
using UnityEngine;
using UnityEngine.XR;

public class MushroomTrackerUIPositioner : MonoBehaviour
{
    [Header("Positioning")]
    [SerializeField] private float distanceFromCamera = 2f;
    [SerializeField] private float heightOffset = 0f; // Height above camera
    [SerializeField] private bool followCamera = true;
    [SerializeField] private bool lookAtCamera = true;

    [Header("Camera Reference")]
    [SerializeField] private Camera targetCamera;
    
    private Canvas canvas;
    private Transform cameraTransform;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        // Find camera
        if (targetCamera == null)
        {
            // Try to find XR camera first using reflection (safer)
            targetCamera = FindXRCamera();
            
            // Fall back to main camera if XR camera not found
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        if (targetCamera != null)
        {
            cameraTransform = targetCamera.transform;
        }

        // Position initially
        if (followCamera)
        {
            UpdatePosition();
        }
    }

    void LateUpdate()
    {
        if (followCamera && cameraTransform != null)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        if (cameraTransform == null || canvas == null) return;

        // Calculate position in front of camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        Vector3 up = cameraTransform.up;

        // Position the UI in front of the camera
        Vector3 targetPosition = cameraTransform.position 
            + forward * distanceFromCamera 
            + up * heightOffset;

        canvas.transform.position = targetPosition;

        // Make UI face the camera
        if (lookAtCamera)
        {
            Vector3 directionToCamera = cameraTransform.position - canvas.transform.position;
            directionToCamera.y = 0; // Keep UI upright (optional - remove this line if you want it to tilt)
            canvas.transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
    }

    // Try to find XR camera using reflection (works even if XR Toolkit isn't available)
    private Camera FindXRCamera()
    {
        // Try to find XROrigin using reflection
        System.Type xrOriginType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XROrigin, Unity.XR.Interaction.Toolkit");
        if (xrOriginType != null)
        {
            UnityEngine.Object xrOrigin = FindObjectOfType(xrOriginType);
            if (xrOrigin != null)
            {
                // Try to get Camera property
                var cameraProperty = xrOriginType.GetProperty("Camera");
                if (cameraProperty != null)
                {
                    Camera xrCamera = cameraProperty.GetValue(xrOrigin) as Camera;
                    if (xrCamera != null)
                    {
                        return xrCamera;
                    }
                }
            }
        }
        return null;
    }

    // Public method to manually position (useful for testing)
    public void PositionInFrontOfCamera(Camera cam, float distance = 2f)
    {
        if (cam == null) return;

        targetCamera = cam;
        cameraTransform = cam.transform;
        distanceFromCamera = distance;
        UpdatePosition();
    }
}

