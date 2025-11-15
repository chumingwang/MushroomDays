using UnityEngine;

public class MushroomInfoUI : MonoBehaviour
{
    public Transform cameraTransform;
    public float distance = 2.2f;
    public float heightOffset = 0f;
    public float horizontalOffset = 0f;
    public void ShowUI()
    {
        gameObject.SetActive(true);

        // position
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Quaternion offsetRotation = Quaternion.Euler(0, horizontalOffset, 0);
        Vector3 rotatedDirection = offsetRotation * forward;

        transform.position = cameraTransform.position
                         + rotatedDirection * distance
                         + Vector3.up * heightOffset;

        transform.rotation = Quaternion.LookRotation(rotatedDirection, Vector3.up);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
