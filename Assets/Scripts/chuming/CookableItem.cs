using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookableItem : MonoBehaviour
{
    public float cookTime = 5f;
    private float cookTimer = 0f;
    private bool isCooking = false;
    private bool isCooked = false;

    private Renderer rend;
    private Material[] mats;
    private Color[] rawColors;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mats = rend.materials;
        rawColors = new Color[mats.Length];

        for (int i = 0; i < mats.Length; i++)
        {
            rawColors[i] = mats[i].color;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cauldron") && !isCooked)
        {
            isCooking = true;
            cookTimer = 0f;
        }
    }

    void Update()
    {
        if (isCooking && !isCooked)
        {
            cookTimer += Time.deltaTime;
            if (cookTimer >= cookTime)
            {
                Cooked();
            }
        }
    }

    void Cooked()
    {
        isCooked = true;
        GetComponent<MushroomProperties>().isCooked = true;
        for (int i = 0; i < mats.Length; i++)
        {
            Color darker = rawColors[i] * 0.7f;
            if (mats[i].HasProperty("_BaseColor"))
                mats[i].SetColor("_BaseColor", darker);
            else if (mats[i].HasProperty("_Color"))
                mats[i].SetColor("_Color", darker);
        }

        Debug.Log($"{gameObject.name} is cooked!");
    }
}
