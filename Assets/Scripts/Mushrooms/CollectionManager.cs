using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    private Dictionary<MushroomType, int> collectedCounts
        = new Dictionary<MushroomType, int>();

    private void Awake()
    {
        Instance = this;

        // Initialize
        foreach (MushroomType type in System.Enum.GetValues(typeof(MushroomType)))
        {
            collectedCounts[type] = 0;
        }
    }

    // Increase count
    public void AddMushroom(MushroomType type)
    {
        collectedCounts[type]++;

        Debug.Log($"Collected {type}: {collectedCounts[type]}");

        // if (collectedCounts[type] >= 5) Unlock(type);
    }
    public bool IsUnlocked(MushroomType type)
    {
        return collectedCounts[type] >= 0;
    }
    public int GetCount(MushroomType type)
    {
        return collectedCounts[type];
    }
}
