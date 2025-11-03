using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class RainOverlayController : MonoBehaviour
{
    public UIDocument uiDoc;
    public Texture2D rainTexture;
    public Vector2 scrollSpeed = new Vector2(0f, -5f);
    [Range(0f, 1f)] public float intensity = 0f;

    VisualElement rainElement;
    Vector2 offset;

    void OnEnable()
    {
        if (!uiDoc) uiDoc = GetComponent<UIDocument>();
        var root = uiDoc.rootVisualElement;

        rainElement = root.Q<VisualElement>("rain-overlay");
        if (rainElement == null)
        {
            rainElement = new VisualElement { name = "rain-overlay" };
            rainElement.style.position = Position.Absolute;
            rainElement.style.left = 0; rainElement.style.top = 0;
            rainElement.style.right = 0; rainElement.style.bottom = 0;
            root.Add(rainElement);
        }

        if (rainTexture != null)
        {
            rainElement.style.backgroundImage = new StyleBackground(rainTexture);
            rainElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Cover);
            rainElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.Repeat);
        }

        rainElement.style.backgroundColor = new Color(1, 1, 1, 0);
        rainElement.style.opacity = 0f;
    }

    void Update()
    {
        if (rainElement == null) return;

        offset.y += scrollSpeed.y * Time.deltaTime;
        rainElement.style.backgroundPositionX =
            new BackgroundPosition(BackgroundPositionKeyword.Left, new Length(50f, LengthUnit.Percent));
        rainElement.style.backgroundPositionY =
            new BackgroundPosition(BackgroundPositionKeyword.Top, new Length(offset.y * 100f, LengthUnit.Percent));

        rainElement.style.opacity = intensity;
    }

    public void SetIntensity(float t) => intensity = Mathf.Clamp01(t);
}
