using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomProperties : MonoBehaviour
{
    public MushroomType type;

    public MushroomData data;

    public float RootOffset = 0f;

    public bool isCollected = false;
    public bool isSliced = false;
    public bool isCooked = false;
}
