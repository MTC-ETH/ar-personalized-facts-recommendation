using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Immersal.AR;
using Immersal.REST;

public class ImmersalManager : MonoBehaviour
{
    public int searchRadius = 150;
    public List<MapInfo> mapsInScene;

    [SerializeField] private GpsManager gpsManager;

    private bool isLocalizing = false;
    private List<int> idsInScene = new List<int>();

    [SerializeField] private DebugText debugText;

    private void Awake()
    {
#if !UNITY_EDITOR
        foreach (MapInfo mapInfo in mapsInScene)
        {
            mapInfo.arMap.enabled = false;
            mapInfo.arMap.gameObject.SetActive(false);
        }
#endif
    }

    private void Start()
    {
        foreach (MapInfo map in mapsInScene)
        {
            idsInScene.Add(map.mapId);
        }

        // Continuously find IDs of nearby maps and use them to update the ID list of the localizer
#if !UNITY_EDITOR
        Invoke("UpdateMapIdsOfLocalizer", 1.5f);
#endif
    }

    // Called when user changes preferences or switches personalization mode
    public void ResetARMapsAndPoiContent()
    {
        foreach (MapInfo mapInfo in mapsInScene)
        {
            if (mapInfo.arMap.enabled)
            {
                mapInfo.arMap.gameObject.GetComponentInChildren<PoiContentManager>().Reset();
            }
        }
    }

    private async void UpdateMapIdsOfLocalizer()
    {
        if (!gpsManager.readyToProvideGpsValues)
        {
            //debugText.Print("GPS Manager not ready yet");
            Invoke("UpdateMapIdsOfLocalizer", 0.5f);
            return;
        }

        List<int> idsOfClosebyMaps = await GetIdsOfClosebyMaps(searchRadius);

        //ID list without IDs that belong to your Immersal account but are not in the scene (e.g. test maps or old maps)      
        List<int> idsOfClosebyMapsInScene = new List<int>();

        foreach (MapInfo mapInfo in mapsInScene)
        {
            bool mapIsCloseby = idsOfClosebyMaps.Contains(mapInfo.mapId);
            
            if (mapInfo.arMap.enabled != mapIsCloseby)
            {
                mapInfo.arMap.gameObject.SetActive(mapIsCloseby);
                mapInfo.arMap.enabled = mapIsCloseby;
            }

            if (mapIsCloseby)
            {
                idsOfClosebyMapsInScene.Add(mapInfo.mapId);

                mapInfo.arMap.mapId = mapInfo.mapId;    // need to set ID manually, because it gets reset to -1 after disabling the AR Map component if there is no .bytes file attached to it
            }

        }

        // Convert list of valid IDs to SKDMapId array
        SDKMapId[] sdkMapIds = new SDKMapId[idsOfClosebyMapsInScene.Count];
        for (int i = 0; i < sdkMapIds.Length; i++)
        {
            sdkMapIds[i] = new SDKMapId() { id = idsOfClosebyMapsInScene[i] };
        }

        // update IDs of localizer
        ARLocalizer.Instance.mapIds = sdkMapIds;

        // Start localizing after getting IDs of closeby maps the first time
        if (!isLocalizing)
        {
            debugText.Print("Start Localization");

            ARLocalizer.Instance.autoStart = true;
            ARLocalizer.Instance.useServerLocalizer = true;
            ARLocalizer.Instance.StartLocalizing();

            isLocalizing = true;
        }

        // Update nearby map IDs in 5 seconds again
        Invoke("UpdateMapIdsOfLocalizer", 5f);
    }

    public async Task<List<int>> GetIdsOfClosebyMaps(int radius)
    {
        List<int> mapIds = new List<int>();

        (double latitude, double longitude) gpsValues = gpsManager.GetGpsValues();

        JobListJobsAsync j = new JobListJobsAsync();

        j.useGPS = true;
        j.latitude = gpsValues.latitude;
        j.longitude = gpsValues.longitude;
        j.radius = radius;

        j.OnResult += (SDKJobsResult result) =>
        {
            foreach (SDKJob job in result.jobs)
            {
                mapIds.Add(job.id);
            }
        };

        await j.RunJobAsync();

        return mapIds;
    }
}

[Serializable]
public struct MapInfo
{
    public int mapId;
    public ARMap arMap;
    public TextAsset pointCloudDataForLocalizationTest;
}
