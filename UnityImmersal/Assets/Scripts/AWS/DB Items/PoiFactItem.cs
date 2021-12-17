using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.DynamoDBv2.DataModel;

[DynamoDBTable(AwsConstants.POI_FACTS_TABLE_NAME)]
public class PoiFactItem
{
    // Key = PoiId + FactId

    [DynamoDBHashKey]
    public int PoiId { get; set; }

    [DynamoDBRangeKey]
    public int FactId { get; set; }

    [DynamoDBProperty]
    public string PoiName { get; set; }

    [DynamoDBProperty]
    public string Fact { get; set; }

    [DynamoDBProperty]
    public Dictionary<string, int> Categories { get; set; }
}