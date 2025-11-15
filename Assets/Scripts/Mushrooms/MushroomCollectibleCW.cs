using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MushroomCollectibleCW : MonoBehaviour
{
    private MushroomProperties props;
    private XRGrabInteractable grab;

    private void Awake()
    {
        props = GetComponent<MushroomProperties>();
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Already collected? Then do nothing.
        if (props.isCollected) return;

        // Mark as collected
        props.isCollected = true;

        // Update the collection manager
        CollectionManager.Instance.AddMushroom(props.data.type);

        Debug.Log("Collected " + props.data.displayName);
    }
}
