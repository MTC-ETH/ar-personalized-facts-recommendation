using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;

public class AwsInitializer : MonoBehaviour
{
    private AmazonDynamoDBClient dynamoDbClient;
    private DynamoDBContext dynamoDbContext;
    private AmazonS3Client s3Client;

    private CognitoAWSCredentials credentials;

    public AmazonDynamoDBClient DynamoDBClient => dynamoDbClient;
    public DynamoDBContext DynamoDbContext => dynamoDbContext;
    public AmazonS3Client S3Client => s3Client;


    private void Awake()
    {
        credentials = new CognitoAWSCredentials(AwsConstants.IDENTITY_POOL_ID, AwsConstants.REGION_ENDPOINT);
        dynamoDbClient = new AmazonDynamoDBClient(credentials, AwsConstants.REGION_ENDPOINT);
        dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        s3Client = new AmazonS3Client(credentials, AwsConstants.REGION_ENDPOINT);
    }
}