using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this is a slightly modified version of Popup.cs from the UltimateCleanGUIPack
public class CustomPopup : MonoBehaviour
{
    public Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);
    public float destroyTime = 0.5f;
    private GameObject m_background;

    private Animator animator;

    public RectTransform canvasRectTransform;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetBool("isHidden", false);
        AddBackground();
    }

    public void Close()
    {
        animator.SetBool("isHidden", true);

        RemoveBackground();
        StartCoroutine(RunPopupDestroy());
    }

    private IEnumerator RunPopupDestroy()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(m_background);
    }

    private void AddBackground()
    {
        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, backgroundColor);
        bgTex.Apply();

        m_background = new GameObject("PopupBackground");
        var image = m_background.AddComponent<Image>();
        var rect = new Rect(0, 0, bgTex.width, bgTex.height);
        var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
        image.material.mainTexture = bgTex;
        image.sprite = sprite;
        var newColor = image.color;
        image.color = newColor;
        image.canvasRenderer.SetAlpha(0.0f);
        image.CrossFadeAlpha(1.0f, 0.4f, false);

        m_background.transform.localScale = new Vector3(1, 1, 1);
        m_background.GetComponent<RectTransform>().sizeDelta = canvasRectTransform.sizeDelta;//GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;
        m_background.transform.SetParent(transform.parent, false);
        m_background.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    private void RemoveBackground()
    {
        var image = m_background.GetComponent<Image>();
        if (image != null)
        {
            image.CrossFadeAlpha(0.0f, 0.2f, false);
        }
    }
}
