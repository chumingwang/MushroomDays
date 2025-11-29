using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : MonoBehaviour
{
    [System.Serializable]
    public class MushroomUIBlock
    {
        public MushroomType type;
        public TMP_Text countText;
        public Button infoButton;
    }

    public MushroomUIBlock[] entries;   // Drag UI objects in Inspector
    public MenuUIManager menuUiManager;         // To open Info UI
    public Button recipe_btn;

    public void Refresh()
    {
        bool recipe_isUnlook = true;
        foreach (var entry in entries)
        {
            int count = CollectionManager.Instance.GetCount(entry.type);

            // Update count text
            entry.countText.text = count + "/1";

            // Unlock button only when count >= 1
            bool unlocked = count >= 1;
            entry.infoButton.gameObject.SetActive(unlocked);

            if (unlocked)
            {
                entry.infoButton.onClick.RemoveAllListeners();
                entry.infoButton.onClick.AddListener(() =>
                {
                    // Show info UI for this type
                    menuUiManager.ShowMushroomInfo(entry.type);
                });
            }
            else {
                recipe_isUnlook = false;
            }
        }
        recipe_btn.gameObject.SetActive(recipe_isUnlook);
    }
}
