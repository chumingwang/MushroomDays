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

    public void Refresh()
    {
        foreach (var entry in entries)
        {
            int count = CollectionManager.Instance.GetCount(entry.type);

            // Update count text
            entry.countText.text = count + "/2";

            // Unlock button only when count >= 2
            bool unlocked = count >= 2;
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
        }
    }
}
