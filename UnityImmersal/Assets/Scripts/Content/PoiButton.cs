using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiButton : MonoBehaviour
{
    [SerializeField] private ContentObject[] contentObjectsToOpen;

    public void OnButtonPressed()
    {
        GetComponent<ContentObject>().Hide();   // Hide button

        StartCoroutine(OpenContentObjectsInSequence());
    }

    private void Update()
    {
        // open all content objects in editor for testing purposes
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnButtonPressed();
        }
#endif
    }

    private IEnumerator OpenContentObjectsInSequence()
    {
        foreach (ContentObject c in contentObjectsToOpen)
        {
            c.PopUp();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
