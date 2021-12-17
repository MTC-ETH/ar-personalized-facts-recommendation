using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoiFacts : MonoBehaviour
{
    [SerializeField] private int poiId;
    [SerializeField] private List<TMP_Text> poiFactTexts;
    [SerializeField] private RecommendationCommunication recommendationCommunication;

    public void GetFactRecommendations()
    {
        recommendationCommunication.StartFactsRecommendationRequest(PlayerPrefs.GetInt("user"), poiId, poiFactTexts.Count, this);
    }

    public void SetFacts(FactRecommendationResponseItem[] items)
    {
        for (int i = 0; i < poiFactTexts.Count; i++)
        {
            poiFactTexts[i].text = items[i].fact;
            poiFactTexts[i].gameObject.GetComponentInChildren<CategoryTags>().SetTags(items[i].categories);
        }
    }
}
