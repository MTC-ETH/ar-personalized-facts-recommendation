using System;
using System.Collections.Generic;
using UnityEngine;

public class UserContextRecommendationManager : MonoBehaviour, IPoiRecommendationProvider
{
    /*[SerializeField] private InstantiatedPoiManager _instantiatedPoiManager;
    [SerializeField] private RecommendationCommunication _recommendationCommunication;
    [SerializeField] private IntInputFieldListener _userIdInputFieldListener;
    public Action<List<PoiData>> RecommendationAvailableEvent { get; set; }
    private string _currentUserId = "0";
    private string _previousUserId = String.Empty;
    private List<PoiData> _previousPoiData;
    private string[] _poisToAddForDemo;

    // Start is called before the first frame update
    void Start()
    {
        _poisToAddForDemo = new []{"La Pasta", "Rheinfelder Bierhalle"};
        _userIdInputFieldListener.IntInputFieldChangedEvent += OnUserIdChangedEvent;
        _recommendationCommunication.RecommendationReceivedEvent += OnRecommendationReceivedEvent;
    }

    private void OnDestroy()
    {
        _userIdInputFieldListener.IntInputFieldChangedEvent -= OnUserIdChangedEvent;
        _recommendationCommunication.RecommendationReceivedEvent -= OnRecommendationReceivedEvent;
    }
    
    private void OnUserIdChangedEvent(int userId)
    {
        _currentUserId = userId.ToString();
    }

    private void OnRecommendationReceivedEvent(RecommendationServerMessage recommendationServerMessage)
    {
        if (!gameObject.activeSelf)
            return;
        
        //todo: remove hack for demo of adding specific poi

        if(_currentUserId == "0" && !recommendationServerMessage.poiNames.Contains(_poisToAddForDemo[0]))
            recommendationServerMessage.poiNames.Insert(0,_poisToAddForDemo[0]);
        else if(_currentUserId == "1" && !recommendationServerMessage.poiNames.Contains(_poisToAddForDemo[1]))
            recommendationServerMessage.poiNames.Insert(0,_poisToAddForDemo[1]);
        
        var poiData = _instantiatedPoiManager.GetPoisFromPoiNames(recommendationServerMessage.poiNames);
        _previousPoiData = poiData;
        RecommendationAvailableEvent?.Invoke(poiData);
    }

    public void StartRecommendation()
    {
        if (_previousUserId == _currentUserId && _previousPoiData != null)
        {
            RecommendationAvailableEvent?.Invoke(_previousPoiData);
            return;
        }

        _recommendationCommunication.StartUserContextRecommendationRequest(_currentUserId);
        _previousUserId = _currentUserId;
    }*/
}