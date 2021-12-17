using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FieldsOfInterestToggles : MonoBehaviour
{
    public GameObject togglePrefab;
    public List<string> categories;

    public Dictionary<string, int> GetFieldsOfInterestValues()
    {
        Dictionary<string, int> values = new Dictionary<string, int>();

        foreach (Transform child in transform)
        {
            values.Add(child.Find("Text").GetComponent<TMP_Text>().text, child.GetComponent<Toggle>().isOn ? 1 : 0);
        }

        return values;
    }

    public void ResetToggles()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<Toggle>().isOn = false;
        }
    }

    public void LoadUserPreferences(UserItem user)
    {
        ResetToggles();

        Dictionary<string, Toggle> toggles = new Dictionary<string, Toggle>();

        foreach (Transform child in transform)
        {
            toggles.Add(child.Find("Text").GetComponent<TMP_Text>().text, child.GetComponent<Toggle>());
        }

        foreach (string category in user.FieldsOfInterest.Keys)
        {
            if (toggles.ContainsKey(category))
            {
                toggles[category].isOn = user.FieldsOfInterest[category] == 1;
            }
        }
    }
}
