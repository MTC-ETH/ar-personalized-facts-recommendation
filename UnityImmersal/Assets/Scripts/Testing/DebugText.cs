using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Temporary solution to display debug messages because IngameDebugConsole is useless due to the URP/AR Foundation bug 
public class DebugText : MonoBehaviour
{
    public TMP_Text text;

    public void Print(string msg)
    {
        text.text += msg + "\n";
    }
}
