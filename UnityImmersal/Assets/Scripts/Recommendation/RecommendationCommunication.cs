using System;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;

[Serializable]
public class RecommendationServerMessage
{
    public List<string> poiNames;
}

[Serializable]
public class UserContextRecommendationRequest
{
    public string userId;
}

[Serializable]
public class FactRecommendationRequest
{
    public int userId;
    public int poiId;
    public int numFacts;
    public bool retrievePersonalizedFacts;
}

[Serializable]
public class FactRecommendationResponse
{
    public FactRecommendationResponseItem[] items;
}

[Serializable]
public class FactRecommendationResponseItem
{
    public string fact;
    public string[] categories;
}


public class RecommendationCommunication : MonoBehaviour
{
    private string _recommendationEndpoint = "http://contextawarear.pythonanywhere.com/";//"http://127.0.0.1:5000/";//"http://129.132.62.163:5000/"; //"http://cribin.pythonanywhere.com/"
    private string _itemSimilarityEndpoint =  "item-similarity-recommendation-query";
    private string _userContextEndpoint = "user-context-recommendation-query";
    private string _factsEndpoint = "poi-facts-recommendation-query";

    public bool retrievePersonalizedFacts;

    private void Awake()
    {
        _itemSimilarityEndpoint = _recommendationEndpoint + _itemSimilarityEndpoint;
        _userContextEndpoint = _recommendationEndpoint + _userContextEndpoint;
        _factsEndpoint = _recommendationEndpoint + _factsEndpoint;

        retrievePersonalizedFacts = true;

        WakeUpServer(); // just a dummy request to prevent cold start delay of server
    }


    public Action<RecommendationServerMessage> RecommendationReceivedEvent { get; set; }
    //Send list of poi id and receive sorted list of poi ids as a response

    public void StartRecommendationRequest(List<string> unrankedPois)
    {
        var recommendationServerMessage = new RecommendationServerMessage();
        recommendationServerMessage.poiNames = unrankedPois;

        RestClient.Post<RecommendationServerMessage>(_recommendationEndpoint, recommendationServerMessage).Then(response =>
        {
            RecommendationReceivedEvent?.Invoke(response);
        });
    }
    
    public void StartItemSimilarityRecommendationRequest(string lastSeenPoi)
    {
        var recommendationServerMessage = new RecommendationServerMessage();
        recommendationServerMessage.poiNames = new List<string> {lastSeenPoi};
        
        RestClient.Post<RecommendationServerMessage>(_itemSimilarityEndpoint, recommendationServerMessage).Then(response =>
        {
            //print("received response: " + response.poiNames[0]);
            RecommendationReceivedEvent?.Invoke(response);
        });
    }
    
    public void StartUserContextRecommendationRequest(string userId)
    {
       var userContextRecommendationReqeust = new UserContextRecommendationRequest();
       userContextRecommendationReqeust.userId = userId;
        
        RestClient.Post<RecommendationServerMessage>(_userContextEndpoint, userContextRecommendationReqeust).Then(response =>
        {
           // print("received response: " + response.poiNames[0]);
            RecommendationReceivedEvent?.Invoke(response);
        });
    }

    public void StartFactsRecommendationRequest(int userId, int poiId, int numFacts, PoiFacts poiFacts)
    {
        var factRecommendationRequest = new FactRecommendationRequest()
        { 
            userId = userId,
            poiId = poiId,
            numFacts = numFacts,
            retrievePersonalizedFacts = retrievePersonalizedFacts
        };

        RestClient.Post<FactRecommendationResponse>(_factsEndpoint, factRecommendationRequest).Then(response =>
        {
            poiFacts.SetFacts(response.items);
        });
        //RestClient.Get("http://127.0.0.1:5000/");
    }

    // Avoid cold start delay of server when retrieving facts
    private void WakeUpServer()
    {
        var dummyRequest = new FactRecommendationRequest()
        {
            userId = PlayerPrefs.GetInt("user"),
            poiId = 1,
            numFacts = 0,
            retrievePersonalizedFacts = retrievePersonalizedFacts
        };

        RestClient.Post<FactRecommendationResponse>(_factsEndpoint, dummyRequest).Then(response =>
        {
            Debug.Log("Sent dummy request to server");
        });
    }
}
