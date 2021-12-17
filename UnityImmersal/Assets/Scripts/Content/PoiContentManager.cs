using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immersal;
using Immersal.AR;

public class PoiContentManager : MonoBehaviour
{
    [SerializeField] private bool waitForGoodPose = true; // only show content when quality of pose is good or excellent
    [SerializeField] private float maxDistance = 150;   // only show content within this distance. if user localizes POI and goes too far away from POI, deactivate POI content
    [SerializeField] private Transform referencePoint;  // used to determine distance between user and POI content, transform of AR Map not always suitable for this
    [Space]
    [SerializeField] private PoiFacts poiFacts;

    private bool poiIsLocalized = false;
    private bool childObjectsEnabled = false;

    private void Start()
    {
        // Call EnableChildObjects in editor for testing purposes
#if UNITY_EDITOR
        poiFacts.GetFactRecommendations();
        Invoke("EnableChildObjects", 1f);
#endif
    }


    private void Update()
    {
        if (poiIsLocalized && Vector3.SqrMagnitude(Camera.main.transform.position - referencePoint.position) <= maxDistance * maxDistance)  // use square magnitude to avoid calculating square roots every frame
        {
            if (!childObjectsEnabled)
            {
                EnableChildObjects();
                childObjectsEnabled = true;
            }
        }
        else if (poiIsLocalized)    // POI localized and outside max distance
        {
            Reset();
        }
    }

    // Called when Poi is outside max distance or when user changes preference settings
    public void Reset()
    {
        if (!poiIsLocalized) return;

        if (childObjectsEnabled)
        {
            DisableChildObjects();
            childObjectsEnabled = false;
        }

        poiIsLocalized = false;
        GetComponentInParent<ARMap>().Reset();  // reset AR Map such that OnFirstLocalization will be called again when user returns to POI
    }

    public void OnFirstLocalization()
    {
        poiFacts.GetFactRecommendations();

        if (waitForGoodPose)
        {
            StartCoroutine(WaitUntilPoseQualityIsGood());
        }
        else
        {
            poiIsLocalized = true;
        }
    }
    
    private IEnumerator WaitUntilPoseQualityIsGood()
    {
        while (ImmersalSDK.Instance.TrackingQuality < 2)
        {
            yield return new WaitForSeconds(0.1f);
        }

        poiIsLocalized = true;
    }

    public void DisableChildObjects()
    {
        ContentObject[] contentObjects = GetComponentsInChildren<ContentObject>();

        foreach (ContentObject obj in contentObjects)
        {
            obj.Hide();
        }
    }

    private void EnableChildObjects()
    {
        ContentObject[] contentObjects = GetComponentsInChildren<ContentObject>();

        foreach (ContentObject obj in contentObjects)
        {
            obj.Activate();
        }
    }
}
