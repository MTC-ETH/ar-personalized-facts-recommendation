using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;

// Currently not used anymore
public class AwsImageManager : MonoBehaviour
{
    private AwsInitializer aws;

    private void Awake()
    {
        aws = GetComponent<AwsInitializer>();

        if (aws == null)
            Debug.LogError("AwsInitializer missing or not on same game object as AwsImageManager!");
    }
    public async Task<byte[]> LoadImageData(string key)
    {
        byte[] data = null;
        GetObjectResponse response = await aws.S3Client.GetObjectAsync(AwsConstants.POI_IMAGE_BUCKET_NAME, key);
        if (response.ResponseStream != null)
        {
            using (StreamReader reader = new StreamReader(response.ResponseStream))
            {
                using (var memstream = new MemoryStream())
                {
                    byte[] buffer = new byte[20000];
                    int bytesRead = 0;
                    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                        memstream.Write(buffer, 0, bytesRead);
                    data = memstream.ToArray();
                }
            }

            Debug.Log($"Poi image " + key + " retrieved");

            return data;
        }
        else
        {
            return null;
        }
    }
}
