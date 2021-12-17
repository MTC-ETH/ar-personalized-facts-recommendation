using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Enter search radius and check which POIs are found
public class NearbyPoisTest : MonoBehaviour
{
    [SerializeField] private ImmersalManager immersalManager;

    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject button;

    [SerializeField] private Transform scrollbarContent;
    [SerializeField] private GameObject scrollViewItemPrefab;
    [SerializeField] private TMP_InputField radiusInputField;

    private void Awake()
    {
        ui.SetActive(false);
        button.SetActive(true);
    }

    public void OnTestButtonClicked()
    {
        ui.SetActive(true);
        button.SetActive(false);

        radiusInputField.text = immersalManager.searchRadius.ToString();

        foreach (Transform child in scrollbarContent)
        {
            Destroy(child.gameObject);
        }
    }

    public async void OnSearchButtonClicked()
    {
        int radius = int.Parse(radiusInputField.text);
        List<int> nearbyIds = await immersalManager.GetIdsOfClosebyMaps(radius);

        foreach (Transform child in scrollbarContent)
        {
            Destroy(child.gameObject);
        }

        foreach (MapInfo info in immersalManager.mapsInScene)
        {
            if (nearbyIds.Contains(info.mapId))
            {
                GameObject nearbyPoiText = Instantiate(scrollViewItemPrefab, scrollbarContent);
                nearbyPoiText.GetComponent<TMP_Text>().text = info.arMap.gameObject.name.Remove(0, 7);
            }
        }
    }

    public void OnCloseButtonClicked()
    {
        ui.SetActive(false);
        button.SetActive(true);
    }
}
