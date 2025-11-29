using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    private List<MushroomProperties> currentItems = new List<MushroomProperties>();

    public List<ItemRequirement> requirements;

    public GameObject winPanel;

    void Start()
    {
        requirements = new List<ItemRequirement>()
        {
            new ItemRequirement {
                type = MushroomType.Boletus,
                neededCount = 1,
                matchCondition = (item) => item.isSliced == false && item.isCooked == true


            },

            new ItemRequirement {
                type = MushroomType.Puffball,
                neededCount = 2,
                matchCondition = (item) => item.isSliced == true && item.isCooked == true
            },
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        var info = other.GetComponent<MushroomProperties>();
        if (info != null)
        {
            currentItems.Add(info);
            CheckRequirements();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var info = other.GetComponent<MushroomProperties>();
        if (info != null)
        {
            currentItems.Remove(info);
            CheckRequirements();
        }
    }

    private void CheckRequirements()
    {
        foreach (var req in requirements)
        {
            int count = currentItems.Count(i => i.type == req.type && req.matchCondition(i));
            Debug.Log(count);
            if (count != req.neededCount)
            {
                Debug.Log("not required item" );
                return;
            }
        }

        var allowedTypes = new HashSet<MushroomType>(requirements.Select(r => r.type));
        foreach (var item in currentItems)
        {
            if (!allowedTypes.Contains(item.type))
            {
                Debug.Log("more item");
                return;
            }
        }

        Debug.Log("correct");
        winPanel.SetActive(true);
    }
}
