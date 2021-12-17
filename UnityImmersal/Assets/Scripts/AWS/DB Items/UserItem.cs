using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.DynamoDBv2.DataModel;

[DynamoDBTable(AwsConstants.USERS_TABLE_NAME)]
public class UserItem
{
    [DynamoDBHashKey]
    public int UserId { get; set; }

    [DynamoDBProperty]
    public string Name { get; set; }

    [DynamoDBProperty]
    public int Age { get; set; }

    [DynamoDBProperty]
    public string Gender { get; set; }

    [DynamoDBProperty]
    public Dictionary<string, int> FieldsOfInterest { get; set; }

    [DynamoDBProperty]
    public Dictionary<string, int> PersonalityTraits { get; set; }
}