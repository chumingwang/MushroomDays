// MushroomTracker.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MushroomTracker : MonoBehaviour
{
    private static MushroomTracker instance;
    public static MushroomTracker Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MushroomTracker>();
                if (instance == null)
                {
                    GameObject go = new GameObject("MushroomTracker");
                    instance = go.AddComponent<MushroomTracker>();
                }
            }
            return instance;
        }
    }

    // Dictionary to track mushrooms by name/type
    private Dictionary<string, int> collectedMushrooms = new Dictionary<string, int>();
    
    // List of all unique mushroom types that have been seen
    private HashSet<string> knownMushroomTypes = new HashSet<string>();

    // Events for UI updates
    public System.Action<string, int> OnMushroomCollected;
    public System.Action OnTrackerUpdated;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterMushroomCollected(MushroomCollectible mushroom)
    {
        if (mushroom == null) return;

        string key = mushroom.DisplayName;
        
        // Add to known types
        knownMushroomTypes.Add(key);

        // Increment count
        if (collectedMushrooms.ContainsKey(key))
        {
            collectedMushrooms[key]++;
        }
        else
        {
            collectedMushrooms[key] = 1;
        }

        // Notify listeners
        OnMushroomCollected?.Invoke(key, collectedMushrooms[key]);
        OnTrackerUpdated?.Invoke();
    }

    public int GetCount(string mushroomName)
    {
        return collectedMushrooms.ContainsKey(mushroomName) ? collectedMushrooms[mushroomName] : 0;
    }

    public Dictionary<string, int> GetAllCollected()
    {
        return new Dictionary<string, int>(collectedMushrooms);
    }

    public List<string> GetAllKnownTypes()
    {
        return knownMushroomTypes.ToList();
    }

    public int GetTotalCollected()
    {
        return collectedMushrooms.Values.Sum();
    }

    public void ClearTracker()
    {
        collectedMushrooms.Clear();
        knownMushroomTypes.Clear();
        OnTrackerUpdated?.Invoke();
    }
}

