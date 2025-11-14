// BasketCollector.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class BasketCollector : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform contentAnchor;   // assign in inspector
    [SerializeField] private Collider basketTrigger;    // assign BasketTrigger collider (isTrigger = true)

    [Header("UI (optional)")]
    [SerializeField] private TextMeshProUGUI countLabel; // world-space TextMeshPro on the basket

    [Header("Rules")]
    [SerializeField] private int capacity = 30;     // max mushrooms
    [SerializeField] private float radius = 0.15f;  // footprint radius inside the basket
    [SerializeField] private float itemHeight = 0.035f; // vertical spacing between layers

    private readonly List<MushroomCollectible> collected = new();

    void Reset()
    {
        // Try to auto-find a trigger on children if not set
        if (!basketTrigger)
        {
            basketTrigger = GetComponentInChildren<Collider>(includeInactive: true);
            if (basketTrigger) basketTrigger.isTrigger = true;
        }
    }

    void OnValidate()
    {
        if (basketTrigger && !basketTrigger.isTrigger)
            basketTrigger.isTrigger = true;
        if (radius < 0.05f) radius = 0.05f;
        if (itemHeight <= 0f) itemHeight = 0.03f;
        if (capacity < 1) capacity = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        TryCollect(other);
    }

    void OnTriggerStay(Collider other)
    {
        // In case it entered while still held and then got released inside.
        TryCollect(other);
    }

    private void TryCollect(Collider other)
    {
        if (collected.Count >= capacity) return;

        var mush = other.GetComponentInParent<MushroomCollectible>();
        if (mush == null) return;

        // Don't �steal� if user is still holding it
        if (mush.grab && mush.grab.isSelected) return;

        // Already collected?
        if (collected.Contains(mush)) return;

        Collect(mush);
    }

    private void Collect(MushroomCollectible mush)
    {
        // Disable physics & collisions
        mush.rb.isKinematic = true;
        mush.rb.useGravity = false;
        if (mush.grab) mush.grab.enabled = false; // prevent re-grab unless you want to allow removing

        foreach (var col in mush.colliders)
        {
            col.enabled = false; // avoids jitter/stack explosions
        }

        // Parent and place neatly
        mush.transform.SetParent(contentAnchor, worldPositionStays: false);
        var idx = collected.Count;
        mush.transform.localPosition = GetPackedLocalPosition(idx);
        mush.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        collected.Add(mush);
        UpdateCountUI();
        
        // Notify mushroom tracker
        MushroomTracker tracker = MushroomTracker.Instance;
        if (tracker != null)
        {
            tracker.RegisterMushroomCollected(mush);
        }
        
        // (Optional) play SFX or haptics here
    }

    // Golden-angle spiral packing with vertical layers
    private Vector3 GetPackedLocalPosition(int index)
    {
        // Create layers in y; ~pi*r^2 per layer estimate
        int perLayer = Mathf.Max(1, Mathf.RoundToInt((Mathf.PI * radius * radius) / (0.005f))); // rough
        int layer = index / perLayer;
        int iInLayer = index % perLayer;

        // Golden angle in radians
        const float golden = 2.39996323f;
        float r = radius * Mathf.Sqrt(iInLayer / (float)perLayer);
        float theta = iInLayer * golden;

        float x = r * Mathf.Cos(theta);
        float z = r * Mathf.Sin(theta);
        float y = layer * itemHeight;

        return new Vector3(x, y, z);
    }

    private void UpdateCountUI()
    {
        if (countLabel) countLabel.text = $"{collected.Count}/{capacity}";
    }

    // Optional: call this to dump/reset the basket
    public void ClearBasket(bool dropWithPhysics = true)
    {
        foreach (var mush in collected)
        {
            if (dropWithPhysics)
            {
                mush.transform.SetParent(null, true);
                foreach (var c in mush.colliders) c.enabled = true;
                mush.rb.isKinematic = false;
                mush.rb.useGravity = true;
                if (mush.grab) mush.grab.enabled = true;
            }
            else
            {
                Destroy(mush.gameObject);
            }
        }
        collected.Clear();
        UpdateCountUI();
    }
}
