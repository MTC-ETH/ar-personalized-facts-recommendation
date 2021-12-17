using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDimmingDisabler : MonoBehaviour
{
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}