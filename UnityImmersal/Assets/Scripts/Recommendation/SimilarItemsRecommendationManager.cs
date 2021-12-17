using System;
using System.Collections.Generic;
using UnityEngine;

public class SimilarItemsRecommendationManager : MonoBehaviour, IPoiRecommendationProvider
{
    /*[SerializeField] private InstantiatedPoiManager _instantiatedPoiManager;
    [SerializeField] private RecommendationCommunication _recommendationCommunication;
    private PoiData _lastSeenPoiData;
    private List<PoiData> _previousPoiData;
    public Action<List<PoiData>> RecommendationAvailableEvent { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _recommendationCommunication.RecommendationReceivedEvent += OnRecommendationReceivedEvent;
    }

    private void OnDestroy()
    {
        _recommendationCommunication.RecommendationReceivedEvent -= OnRecommendationReceivedEvent;
    }

    private void OnRecommendationReceivedEvent(RecommendationServerMessage recommendationServerMessage)
    {
        if (!gameObject.activeSelf)
            return;

        var poiData = _instantiatedPoiManager.GetPoisFromPoiNames(recommendationServerMessage.poiNames);
        _previousPoiData = poiData;
        RecommendationAvailableEvent?.Invoke(poiData);
    }

    public void StartRecommendation()
    {
        if (LastSeenPoiProviderSingleton.Instance.LastSeenPoiData == null)
        {
            print("No poi has been viewed by the user. Aborting item similarity recommendation");
            return;
        }

        if (_lastSeenPoiData != LastSeenPoiProviderSingleton.Instance.LastSeenPoiData)
        {
            _lastSeenPoiData = LastSeenPoiProviderSingleton.Instance.LastSeenPoiData;
            _recommendationCommunication.StartItemSimilarityRecommendationRequest(_lastSeenPoiData.name);
        }
        else if (_previousPoiData != null)
            RecommendationAvailableEvent?.Invoke(_previousPoiData);
    }*/
}