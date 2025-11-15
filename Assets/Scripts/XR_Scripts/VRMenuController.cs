using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRMenuController : MonoBehaviour
{
    public Transform cameraTransform;
    public float distance = 2.2f; 
    public float heightOffset = 0f;
    public GameObject rayInteractor;

    public void PositionInFront()
    {
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        transform.position = cameraTransform.position
                             + forward * distance
                             + Vector3.up * heightOffset;

        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    public void ToggleMenu()
    {
        bool newState = !gameObject.activeSelf;
        gameObject.SetActive(newState);

        if (newState)
        {
            PositionInFront();
        }

        rayInteractor.SetActive(newState);
    }
}
