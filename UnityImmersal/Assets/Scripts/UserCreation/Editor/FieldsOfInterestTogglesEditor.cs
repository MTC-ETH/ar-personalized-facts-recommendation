using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(FieldsOfInterestToggles))]
public class FieldsOfInterestTogglesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FieldsOfInterestToggles toggles = (FieldsOfInterestToggles)target;
        if (GUILayout.Button("Apply Categories"))
        {
            ApplyCategories(toggles);
        }
    }

    private void ApplyCategories(FieldsOfInterestToggles toggles)
    {
        // Remove current toggles
        while (toggles.gameObject.transform.childCount > 0)
        {
            DestroyImmediate(toggles.gameObject.transform.GetChild(0).gameObject);
        }

        // Create toggles for new categories
        foreach (string category in toggles.categories)
        {
            GameObject toggle = Instantiate(toggles.togglePrefab, toggles.transform);
            toggle.name = category + "Toggle";
            toggle.transform.Find("Text").GetComponent<TMP_Text>().text = category;
        }
    }
}
