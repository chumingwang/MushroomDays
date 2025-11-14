// MushroomTrackerUISetup.cs
// This is an editor helper script to quickly set up the mushroom tracking UI
// Attach this to a GameObject in your scene and click the button in the inspector to auto-create the UI

using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MushroomTrackerUISetup : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Create Mushroom Tracker UI")]
    public void CreateMushroomTrackerUI()
    {
        // Get default TMP font asset
        TMP_FontAsset defaultFont = null;
        
        // Try to get from TMP Settings (most reliable)
        if (TMP_Settings.instance != null)
        {
            defaultFont = TMP_Settings.defaultFontAsset;
        }
        
        // If still null, try to find any existing TextMeshProUGUI in the scene
        if (defaultFont == null)
        {
            TextMeshProUGUI existingText = FindObjectOfType<TextMeshProUGUI>();
            if (existingText != null && existingText.font != null)
            {
                defaultFont = existingText.font;
            }
        }
        
        // Last resort: try to load from resources
        if (defaultFont == null)
        {
            defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }
        
        if (defaultFont == null)
        {
            Debug.LogWarning("Could not find a default TMP font. TextMeshPro components may not display correctly. Please assign fonts manually in the inspector.");
        }

        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject canvasObj = null;
        if (canvas == null)
        {
            canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Set up Canvas for VR (world space, reasonable size)
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(400, 600);
            canvasRect.localScale = new Vector3(0.001f, 0.001f, 0.001f); // Scale down for world space
        }
        else
        {
            canvasObj = canvas.gameObject;
        }

        // Create UI Panel
        GameObject panelObj = new GameObject("MushroomTrackerPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black background

        VerticalLayoutGroup panelLayout = panelObj.AddComponent<VerticalLayoutGroup>();
        panelLayout.padding = new RectOffset(10, 10, 10, 10);
        panelLayout.spacing = 10f;
        panelLayout.childControlWidth = true;
        panelLayout.childControlHeight = false;
        panelLayout.childForceExpandWidth = true;

        // Create Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) titleText.font = defaultFont;
        titleText.text = "Mushroom Collection";
        titleText.fontSize = 24f;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(0, 40f);

        // Create Total Count Text
        GameObject totalObj = new GameObject("TotalCount");
        totalObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI totalText = totalObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) totalText.font = defaultFont;
        totalText.text = "Total: 0";
        totalText.fontSize = 20f;
        totalText.color = Color.yellow;
        totalText.alignment = TextAlignmentOptions.Center;

        RectTransform totalRect = totalObj.GetComponent<RectTransform>();
        totalRect.sizeDelta = new Vector2(0, 30f);

        // Create Content Area (ScrollView)
        GameObject scrollViewObj = new GameObject("ScrollView");
        scrollViewObj.transform.SetParent(panelObj.transform, false);
        ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        RectTransform scrollRectTransform = scrollViewObj.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0, 0);
        scrollRectTransform.anchorMax = new Vector2(1, 1);
        scrollRectTransform.sizeDelta = Vector2.zero;
        scrollRectTransform.anchoredPosition = Vector2.zero;

        // Create Viewport
        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollViewObj.transform, false);
        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0);
        Mask viewportMask = viewportObj.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.anchoredPosition = Vector2.zero;

        scrollRect.viewport = viewportRect;

        // Create Content
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportObj.transform, false);
        VerticalLayoutGroup contentLayout = contentObj.AddComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 5f;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        contentLayout.padding = new RectOffset(5, 5, 5, 5);

        ContentSizeFitter contentFitter = contentObj.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);
        contentRect.anchoredPosition = Vector2.zero;

        scrollRect.content = contentRect;

        // Create Toggle Button
        GameObject buttonObj = new GameObject("ToggleButton");
        buttonObj.transform.SetParent(panelObj.transform, false);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);

        Button button = buttonObj.AddComponent<Button>();

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 40f);

        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        if (defaultFont != null) buttonText.font = defaultFont;
        buttonText.text = "Hide/Show";
        buttonText.fontSize = 18f;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;

        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        buttonTextRect.anchoredPosition = Vector2.zero;

        // Add MushroomTrackerUI component
        MushroomTrackerUI trackerUI = panelObj.AddComponent<MushroomTrackerUI>();
        
        // Set up references
        trackerUI.uiPanel = panelObj;
        trackerUI.contentParent = contentObj.transform;
        trackerUI.totalCountText = totalText;
        trackerUI.toggleButton = button;

        // Set button onClick
        button.onClick.AddListener(() => trackerUI.ToggleUI());

        // Add UI Positioner component to automatically position in front of camera
        MushroomTrackerUIPositioner positioner = canvasObj.AddComponent<MushroomTrackerUIPositioner>();
        
        // Try to find camera and position UI
        Camera mainCamera = Camera.main;
        
        // Try to find XR camera using reflection (safer - works even if XR Toolkit isn't available)
        System.Type xrOriginType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XROrigin, Unity.XR.Interaction.Toolkit");
        if (xrOriginType != null)
        {
            UnityEngine.Object xrOrigin = FindObjectOfType(xrOriginType);
            if (xrOrigin != null)
            {
                var cameraProperty = xrOriginType.GetProperty("Camera");
                if (cameraProperty != null)
                {
                    Camera xrCamera = cameraProperty.GetValue(xrOrigin) as Camera;
                    if (xrCamera != null)
                    {
                        mainCamera = xrCamera;
                    }
                }
            }
        }

        // Position canvas in world space
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            
            if (mainCamera != null)
            {
                // Position in front of camera
                Vector3 forward = mainCamera.transform.forward;
                Vector3 position = mainCamera.transform.position + forward * 2f + Vector3.up * 0.5f;
                canvasRect.position = position;
                canvasRect.rotation = Quaternion.LookRotation(-forward);
            }
            else
            {
                // Default position if no camera found
                canvasRect.position = new Vector3(0, 1.6f, 2f); // Eye level, 2 meters away
                canvasRect.rotation = Quaternion.identity;
            }
        }

        Debug.Log("Mushroom Tracker UI created successfully! The UI is set up and ready to use.");
        if (mainCamera == null)
        {
            Debug.LogWarning("No camera found. The UI positioner will try to find a camera at runtime.");
        }
        EditorUtility.SetDirty(panelObj);
    }
#endif
}

