using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class MenuUIManager : MonoBehaviour
{
    [Header("Player")]
    public Transform cameraTransform;

    [Header("Position Settings")]
    public float distance = 2.2f; 
    public float heightOffset = 0f;

    [Header("Ray Interactors")]
    public GameObject rayInteractor;

    [Header("UI Panels")]
    public GameObject mainMenuUI;
    public GameObject collectionUI;
    public GameObject infoUI;

    public MushroomInfoUI infoLayer;

    public void ToggleMenu()
    {
        bool newState = !gameObject.activeSelf;
        gameObject.SetActive(newState);

        if (newState)
        {
            ShowMainMenuUI();   // always start with main menu
            PositionInFront();
        }

        rayInteractor.SetActive(newState);
    }
    
    //Position the whole menu canvas in front of the player.
    public void PositionInFront()
    {
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        transform.position = cameraTransform.position
                             + forward * distance
                             + Vector3.up * heightOffset;

        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    public void ShowMainMenuUI()
    {
        mainMenuUI.SetActive(true);
        collectionUI.SetActive(false);
        infoUI.SetActive(false);
    }

    public void ShowCollectionUI()
    {
        // Keep the menu canvas unchanged (only swap the panels)
        mainMenuUI.SetActive(false);
        infoUI.SetActive(false);
        collectionUI.SetActive(true);

        // Refresh collected data
        collectionUI.GetComponent<CollectionUI>().Refresh();
    }

    public void ShowMushroomInfo(MushroomType type)
    {
        mainMenuUI.SetActive(false);
        collectionUI.SetActive(false);
        infoUI.SetActive(true);

        // Call MushroomData directly (you already have ScriptableObjects)
        infoLayer.ShowInfo(GetData(type));
    }

    private MushroomData GetData(MushroomType type)
    {
        // simplest: drag all ScriptableObjects into an array
        foreach (var d in dataArray)
            if (d.type == type) return d;

        return null;
    }

    public MushroomData[] dataArray; // drag MushroomData SO objects here
}
