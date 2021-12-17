using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immersal;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/*
 * Some of the GPS code is copied from the MapperBase.cs class from the mapping scene of the samples project. 
 */

public class GpsManager : MonoBehaviour
{
    private double latitude = 0.0;
    private double longitude = 0.0;

    // used to tell user to enable GPS if it is disabled
    [SerializeField] private GameObject gpsNotification;
    private bool gpsEnabledPreviously = false;

    [HideInInspector] public bool readyToProvideGpsValues = false;

    [SerializeField] private DebugText debugText;

    private void Awake()
    {
        gpsNotification.SetActive(false);
    }

    private void Start()
    {
        // Check if GPS is enabled twice a second
        InvokeRepeating("CheckIfGpsEnabled", 0.1f, 0.5f);
    }

    private void CheckIfGpsEnabled()
    {
        bool gpsEnabled = Input.location.isEnabledByUser;

        if (!gpsEnabled)
        {
            readyToProvideGpsValues = false;
        }

        // don't show GPS notification in editor for testing purposes
#if !UNITY_EDITOR
        gpsNotification.SetActive(!gpsEnabled);
#endif

        // if GPS was not enabled previously but is now, start GPS
        if (!gpsEnabledPreviously && gpsEnabled)
        {
            StartGPS();
        }

        gpsEnabledPreviously = gpsEnabled;
    }

    private void StartGPS()
    {
#if UNITY_IOS
            StartCoroutine(EnableLocationServices());
#elif PLATFORM_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Location permission OK");
            debugText.Print("Location permission OK");
            StartCoroutine(EnableLocationServices());
        }
        else
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            StartCoroutine(WaitForLocationPermission());
        }
#endif
    }

#if PLATFORM_ANDROID
    private IEnumerator WaitForLocationPermission()
    {
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return null;
        }

        Debug.Log("Location permission OK");
        debugText.Print("Location permission OK");
        StartCoroutine(EnableLocationServices());
        yield return null;
    }
#endif

    private IEnumerator EnableLocationServices()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled");
            debugText.Print("Location services not enabled");
            yield break;
        }

        // Start service before querying location
#if (UNITY_IOS || PLATFORM_ANDROID) && !UNITY_EDITOR
        NativeBindings.StartLocation();
#else
        Input.location.Start(0.001f, 0.001f);
#endif

        // Wait until service initializes
        int maxWait = 10;
#if (UNITY_IOS || PLATFORM_ANDROID) && !UNITY_EDITOR
        while (!NativeBindings.LocationServicesEnabled() && maxWait > 0)
#else
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
#endif
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            debugText.Print("Timed out");

            // Try it again
            Invoke("StartGPS", 0.5f);

            yield break;
        }

        // Connection has failed
#if (UNITY_IOS || PLATFORM_ANDROID) && !UNITY_EDITOR
            if (!NativeBindings.LocationServicesEnabled())
#else
        if (Input.location.status == LocationServiceStatus.Failed)
#endif
        {
            Debug.Log("Unable to determine device location");
            debugText.Print("Unable to determine device location");
            yield break;
        }

#if (UNITY_IOS || PLATFORM_ANDROID) && !UNITY_EDITOR
            if (NativeBindings.LocationServicesEnabled())
#else
        if (Input.location.status == LocationServiceStatus.Running)
#endif
        {
            Debug.Log("Tracking geolocation");
            debugText.Print("Tracking geolocation");

            readyToProvideGpsValues = true;
        }
    }

    public (double, double) GetGpsValues()
    {
#if (UNITY_IOS || PLATFORM_ANDROID) && !UNITY_EDITOR
        latitude = NativeBindings.GetLatitude();
        longitude = NativeBindings.GetLongitude();
#else
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
#endif
        return (latitude, longitude);
    }
}
