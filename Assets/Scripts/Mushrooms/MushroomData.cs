using UnityEngine;

[CreateAssetMenu(fileName = "MushroomData", menuName = "Mushrooms/MushroomData")]
public class MushroomData : ScriptableObject
{
    public MushroomType type;

    public string displayName;
    // public Sprite icon;
    [TextArea] public string description;
}
