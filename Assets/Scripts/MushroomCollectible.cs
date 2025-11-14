// MushroomCollectible.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class MushroomCollectible : MonoBehaviour
{
    [Header("Mushroom Info")]
    [SerializeField] private string mushroomName = "Mushroom";
    [SerializeField] private string mushroomType = "Common";
    
    [Header("Components")]
    public Rigidbody rb;
    public XRGrabInteractable grab;
    public Collider[] colliders;

    public string MushroomName => mushroomName;
    public string MushroomType => mushroomType;
    public string DisplayName => string.IsNullOrEmpty(mushroomName) ? mushroomType : mushroomName;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        grab = GetComponent<XRGrabInteractable>();
        if (grab == null)
            grab = GetComponentInChildren<XRGrabInteractable>();

        colliders = GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            // Add a basic collider if none exist
            var col = gameObject.AddComponent<BoxCollider>();
            colliders = new Collider[] { col };
        }
    }

    void Reset()
    {
        // Auto-setup if name is empty
        if (string.IsNullOrEmpty(mushroomName))
        {
            mushroomName = gameObject.name.Replace("(Clone)", "").Trim();
        }
    }
}

