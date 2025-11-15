using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : MonoBehaviour
{
    public TMP_Text collectionText;
    // Or TMP_Text if using TextMeshPro

    public void Refresh()
    {
        StringBuilder sb = new StringBuilder();

        foreach (MushroomType type in System.Enum.GetValues(typeof(MushroomType)))
        {
            int count = CollectionManager.Instance.GetCount(type);
            bool unlocked = CollectionManager.Instance.IsUnlocked(type);

            sb.AppendLine($"{type}: {count}/5 {(unlocked ? "(Unlocked)" : "")}");
        }

        collectionText.text = sb.ToString();
    }
}
