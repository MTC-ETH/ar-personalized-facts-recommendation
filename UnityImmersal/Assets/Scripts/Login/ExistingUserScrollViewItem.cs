using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UltimateClean;

public class ExistingUserScrollViewItem : MonoBehaviour
{
    [SerializeField] private TMP_Text userNameText;
    [HideInInspector] public int userId;
    [HideInInspector] public string userName;

    public void SetUpScrollViewItem(UserIdNamePair pair)
    {
        userId = pair.userId;
        userName = pair.userName;

        userNameText.text = userName;
    }

    public void OnSelectUserButtonClicked()
    {
        GameObject.Find("LoginManager").GetComponent<LoginManager>().LoadExistingUser(userId);
    }

    public void OnDeleteUserButtonClicked()
    {
        GameObject.Find("LoginManager").GetComponent<LoginUIManager>().ShowDeletionPopUp(userId, userName);
    }
}
