using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CategoryTags : MonoBehaviour
{
    // Category tags
    [SerializeField] private GameObject politics;
    [SerializeField] private GameObject history;
    [SerializeField] private GameObject economy;
    [SerializeField] private GameObject art;
    [SerializeField] private GameObject religion;
    [SerializeField] private GameObject technology;

    public void SetTags(string[] categories)
    {
        politics.SetActive(categories.Contains("Politics"));
        history.SetActive(categories.Contains("History"));
        economy.SetActive(categories.Contains("Economy"));
        art.SetActive(categories.Contains("Art"));
        religion.SetActive(categories.Contains("Religion"));
        technology.SetActive(categories.Contains("Technology"));
    }
}
