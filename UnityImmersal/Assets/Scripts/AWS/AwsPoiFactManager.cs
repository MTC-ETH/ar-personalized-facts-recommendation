using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AwsPoiFactManager : MonoBehaviour
{
    private AwsInitializer aws;

    private void Awake()
    {
        aws = GetComponent<AwsInitializer>();

        if (aws == null)
            Debug.LogError("AwsInitializer missing or not on same game object as AwsPoiTextManager!");
    }

    public async void SavePoiFact(PoiFactItem poiFact)
    {
        await aws.DynamoDbContext.SaveAsync(poiFact);
        //Debug.Log($"POI Fact {poiFact.PoiId}-{poiFact.FactId} saved");
    }

    public async Task<PoiFactItem> LoadPoiFact(int poiId, int factId)
    {
        PoiFactItem poiFact = await aws.DynamoDbContext.LoadAsync<PoiFactItem>(poiId, factId);
        //Debug.Log($"POI Fact {poiFact.PoiId}-{poiFact.FactId} retrieved");
        return poiFact;
    }
}
