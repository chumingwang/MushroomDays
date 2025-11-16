using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MushroomInfoUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Image icon;

    public void ShowInfo(MushroomData data)
    {
        nameText.text = data.displayName;
        descriptionText.text = data.description;
        // icon.sprite = data.icon;
    }
}
