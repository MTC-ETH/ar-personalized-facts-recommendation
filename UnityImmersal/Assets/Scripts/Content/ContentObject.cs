using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentObject : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool openAtStart;

    // called by PoiContentManager when POI is localized
    public void Activate()
    {
        if (openAtStart)
        {
            PopUp();
        }
    }

    public void PopUp()
    {
        animator.SetBool("isHidden", false);
    }

    public void Hide()
    {
        animator.SetBool("isHidden", true);
    }
}
