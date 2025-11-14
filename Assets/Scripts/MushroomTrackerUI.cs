// MushroomTrackerUI.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MushroomTrackerUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiPanel;
    public Transform contentParent; // Parent for mushroom entries
    public GameObject mushroomEntryPrefab; // Prefab for each mushroom type entry
    public TextMeshProUGUI totalCountText;
    public Button toggleButton; // Optional button to show/hide UI

    [Header("Settings")]
    [SerializeField] private bool startVisible = true;

    private Dictionary<string, GameObject> entryObjects = new Dictionary<string, GameObject>();
    private MushroomTracker tracker;

    void Start()
    {
        tracker = MushroomTracker.Instance;
        
        // Subscribe to tracker events
        tracker.OnMushroomCollected += UpdateMushroomEntry;
        tracker.OnTrackerUpdated += RefreshUI;

        // Setup UI visibility
        if (uiPanel != null)
        {
            uiPanel.SetActive(startVisible);
        }

        // Setup toggle button if provided
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleUI);
        }

        // Initial UI refresh
        RefreshUI();
    }

    void OnDestroy()
    {
        if (tracker != null)
        {
            tracker.OnMushroomCollected -= UpdateMushroomEntry;
            tracker.OnTrackerUpdated -= RefreshUI;
        }
    }

    public void ToggleUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
    }

    public void ShowUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
    }

    public void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    private void UpdateMushroomEntry(string mushroomName, int count)
    {
        if (!entryObjects.ContainsKey(mushroomName))
        {
            CreateMushroomEntry(mushroomName);
        }

        UpdateEntryDisplay(mushroomName, count);
        UpdateTotalCount();
    }

    private void CreateMushroomEntry(string mushroomName)
    {
        if (contentParent == null) return;

        GameObject entry;
        
        if (mushroomEntryPrefab != null)
        {
            entry = Instantiate(mushroomEntryPrefab, contentParent);
        }
        else
        {
            // Create a simple entry if no prefab is provided
            entry = CreateSimpleEntry(mushroomName);
        }

        entry.name = $"Entry_{mushroomName}";
        entryObjects[mushroomName] = entry;
    }

    private GameObject CreateSimpleEntry(string mushroomName)
    {
        // Get default font
        TMP_FontAsset defaultFont = GetDefaultFont();

        GameObject entry = new GameObject($"Entry_{mushroomName}");
        entry.transform.SetParent(contentParent, false);

        // Add horizontal layout
        HorizontalLayoutGroup layout = entry.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // Add RectTransform setup
        RectTransform rect = entry.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 30f);

        // Create name text
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(entry.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) nameText.font = defaultFont;
        nameText.text = mushroomName;
        nameText.fontSize = 16f;
        nameText.color = Color.white;

        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(0.7f, 1);
        nameRect.sizeDelta = Vector2.zero;
        nameRect.anchoredPosition = Vector2.zero;

        // Create count text
        GameObject countObj = new GameObject("Count");
        countObj.transform.SetParent(entry.transform, false);
        TextMeshProUGUI countText = countObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) countText.font = defaultFont;
        countText.text = "0";
        countText.fontSize = 16f;
        countText.color = Color.yellow;
        countText.alignment = TextAlignmentOptions.Right;

        RectTransform countRect = countObj.GetComponent<RectTransform>();
        countRect.anchorMin = new Vector2(0.7f, 0);
        countRect.anchorMax = new Vector2(1, 1);
        countRect.sizeDelta = Vector2.zero;
        countRect.anchoredPosition = Vector2.zero;

        return entry;
    }

    private void UpdateEntryDisplay(string mushroomName, int count)
    {
        if (!entryObjects.ContainsKey(mushroomName)) return;

        GameObject entry = entryObjects[mushroomName];
        
        // Try to find count text component
        TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 2)
        {
            // Assume last text is the count
            texts[texts.Length - 1].text = count.ToString();
        }
        else
        {
            // Fallback: search for "Count" child
            Transform countTransform = entry.transform.Find("Count");
            if (countTransform != null)
            {
                TextMeshProUGUI countText = countTransform.GetComponent<TextMeshProUGUI>();
                if (countText != null)
                {
                    countText.text = count.ToString();
                }
            }
        }
    }

    private void UpdateTotalCount()
    {
        if (totalCountText != null && tracker != null)
        {
            totalCountText.text = $"Total: {tracker.GetTotalCollected()}";
        }
    }

    private void RefreshUI()
    {
        if (tracker == null) return;

        var allCollected = tracker.GetAllCollected();
        
        // Update or create entries for all collected mushrooms
        foreach (var kvp in allCollected)
        {
            if (!entryObjects.ContainsKey(kvp.Key))
            {
                CreateMushroomEntry(kvp.Key);
            }
            UpdateEntryDisplay(kvp.Key, kvp.Value);
        }

        // Remove entries for mushrooms that are no longer collected (if basket is cleared)
        var keysToRemove = entryObjects.Keys.Where(k => !allCollected.ContainsKey(k)).ToList();
        foreach (var key in keysToRemove)
        {
            Destroy(entryObjects[key]);
            entryObjects.Remove(key);
        }

        UpdateTotalCount();
    }

    private TMP_FontAsset GetDefaultFont()
    {
        // Try to get font from TMP Settings
        if (TMP_Settings.instance != null)
        {
            return TMP_Settings.defaultFontAsset;
        }

        // Try to find any existing TextMeshProUGUI in the scene to get its font
        TextMeshProUGUI existingText = FindObjectOfType<TextMeshProUGUI>();
        if (existingText != null && existingText.font != null)
        {
            return existingText.font;
        }

        // Last resort: try to load from resources
        return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
    }
}

