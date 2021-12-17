using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCanvasToggler : MonoBehaviour
{
    [SerializeField] private GameObject testingCanvas;

    private void Awake()
    {
        testingCanvas.SetActive(false);
    }

    public void ToggleTestingCanvas()
    {
        testingCanvas.SetActive(!testingCanvas.activeSelf);
    }
}
