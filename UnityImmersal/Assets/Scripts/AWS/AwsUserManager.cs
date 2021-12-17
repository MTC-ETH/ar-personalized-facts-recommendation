using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;

public class AwsUserManager : MonoBehaviour
{
    private AwsInitializer aws;

    private void Awake()
    {
        aws = GetComponent<AwsInitializer>();

        if (aws == null)
            Debug.LogError("AwsInitializer missing or not on same game object as AwsUserManager!");
    }

    public async void SaveUser(UserItem user)
    {
        await aws.DynamoDbContext.SaveAsync(user);
        //Debug.Log($"User {user.UserId} saved");
    }

    public async void DeleteUser(int userId)
    {
        DeleteItemRequest request = new DeleteItemRequest
        {
            TableName = AwsConstants.USERS_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>() 
            {
                { "UserId", new AttributeValue { N = userId.ToString() } }
            }
        };

        await aws.DynamoDBClient.DeleteItemAsync(request);
    }

    public async Task<UserItem> LoadUser(int userId)
    {
        UserItem user = await aws.DynamoDbContext.LoadAsync<UserItem>(userId);
        //Debug.Log($"User {user.UserId} retrieved");
        return user;
    }

    public async Task<List<UserIdNamePair>> LoadAllUserNamesAndIds()
    {
        ScanRequest request = new ScanRequest
        {
            TableName = AwsConstants.USERS_TABLE_NAME,
            AttributesToGet = new List<string> { "UserId", "Name"}
        };

        ScanResponse response = await aws.DynamoDBClient.ScanAsync(request);

        List<UserIdNamePair> userIdNamePairs = new List<UserIdNamePair>();

        foreach (Dictionary<string, AttributeValue> item in response.Items)
        {
            UserIdNamePair pair = new UserIdNamePair
            {
                userId = int.Parse(item["UserId"].N),
                userName = item["Name"].S
            };

            userIdNamePairs.Add(pair);
        }   

        return userIdNamePairs;
    }
}

public struct UserIdNamePair
{
    public int userId;
    public string userName;
}