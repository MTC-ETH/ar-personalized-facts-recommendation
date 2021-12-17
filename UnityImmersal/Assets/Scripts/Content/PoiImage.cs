using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PoiImage : MonoBehaviour
{
    [SerializeField] private string imageKey;
    [SerializeField] private Image image;
 
    [SerializeField] private RectTransform borderRect;
    [SerializeField] private float borderOffset;

    private void Awake()
    {
        Texture2D.allowThreadedTextureCreation = true;
    }

    private void Start()
    {
        StartCoroutine(SetImage());
    }

    private IEnumerator SetImage()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(AwsConstants.IMAGE_BUCKET_URL + imageKey))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
                AdjustBorder(texture.width, texture.height);
            }
        }
    }

    private void AdjustBorder(int width, int height)
    {
        float w, h;

        float imageW = image.rectTransform.sizeDelta.x;
        float imageH = image.rectTransform.sizeDelta.y;

        if (width > height)
        {
            w = imageW;
            h = (float)height / (float)width * imageW;
        }
        else
        {
            h = imageH;
            w = (float)width / (float)height * imageH;
        }

        borderRect.sizeDelta = new Vector2(w + borderOffset, h + borderOffset);
    }
}
