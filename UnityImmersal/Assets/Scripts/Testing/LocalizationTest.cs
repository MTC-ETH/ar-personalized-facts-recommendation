using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immersal.AR;

// Check accuracy of localization by visualizing point cloud
public class LocalizationTest : MonoBehaviour
{
    [SerializeField] private ImmersalManager immersalManager;
    private bool pointCloudsVisible;

    private void Awake()
    {
        pointCloudsVisible = false;
    }

    public void TogglePointCloudRenderMode()
    {
        pointCloudsVisible = !pointCloudsVisible;

        foreach (MapInfo mapInfo in immersalManager.mapsInScene)
        {
            if (pointCloudsVisible)
            {
                mapInfo.arMap.renderMode = ARMap.RenderMode.EditorAndRuntime;

                if (mapInfo.arMap.mapFile == null)
                {
                    mapInfo.arMap.mapFile = mapInfo.pointCloudDataForLocalizationTest;
                    mapInfo.arMap.LoadMap();
                }
            }
            else
            {
                mapInfo.arMap.renderMode = ARMap.RenderMode.DoNotRender;
            }
        }
    }

    public void ChangePointSize(float size)
    {
        ARMap.pointSize = size;
    }
}
