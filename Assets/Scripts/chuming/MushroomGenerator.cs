using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MushroomGenerator : MonoBehaviour
{
    public GameObject[] mushrooms;
    public int count = 100;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    public Terrain terrain;


    // Start is called before the first frame update
    void Start()
    {
        GenerateMushrooms();
    }

    void GenerateMushrooms()
    {
        if (mushrooms == null || terrain == null)
        {
            Debug.LogWarning("Mushroom prefab or terrain is not assigned.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        for (int i = 0; i < count; i++)
        {
            // Generate random position within terrain bounds
            float x = Random.Range(0, terrainSize.x);
            float z = Random.Range(0, terrainSize.z);

            float worldX = terrainPos.x + x;
            float worldZ = terrainPos.z + z;

            // Get the terrain height at this position
            float y = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainPos.y;

            // Convert local terrain position to world position
            Vector3 spawnPos = new Vector3(worldX, y, worldZ);

            GameObject mushroomPrefab = mushrooms[Random.Range(0, mushrooms.Length)];

            // Instantiate the mushroom prefab
            GameObject mushroom = Instantiate(mushroomPrefab, spawnPos, Quaternion.identity);

            mushroom.AddComponent<MushroomPhysics>();
            mushroom.AddComponent<MushroomCollectibleCW>();

            // Apply random Y rotation
            float randomYRotation = Random.Range(0f, 360f);
            mushroom.transform.Rotate(0f, randomYRotation, 0f, Space.Self);

            // Apply random scale
            float randomScale = Random.Range(minScale, maxScale);
            mushroom.transform.localScale *= randomScale;

            // Apply root offset (after scaling)
            MushroomProperties properties = mushroom.GetComponent<MushroomProperties>();
            if (properties != null)
            {
                mushroom.transform.position += Vector3.up * properties.RootOffset * randomScale;
            }

            // Disable gravity initially
            Rigidbody rb = mushroom.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
            }

            MeshCollider collider = mushroom.GetComponent<MeshCollider>();
            collider.isTrigger = true;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
