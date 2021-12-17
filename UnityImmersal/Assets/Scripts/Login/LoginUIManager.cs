using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginUIManager : MonoBehaviour
{
    [SerializeField] private LoginManager loginManager;
    [SerializeField] private UserCreationManager userCreationManager;
    [Space]
    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject loadUserUI;
    [SerializeField] private GameObject createUserUI;
    [Space]
    [SerializeField] private GameObject loginAsLastUsedUserButton;
    [SerializeField] private GameObject noExistingUsersNotifications;
    [Space]
    [SerializeField] private Transform existingUserScrollViewContentTransform;
    [SerializeField] private GameObject existingUserScrollViewItemPrefab;
    [Space]
    [SerializeField] private CustomPopup deletionPopUp;
    [SerializeField] private TMP_Text deletionText;
    private int idOfUserToDelete = -1;

    void Awake()
    {
        loginAsLastUsedUserButton.SetActive(false);

        ShowHomeMenu();
    }

    public void SetUpLoginAsLastUsedUserButton(UserItem lastUsedUser)
    {
        if (lastUsedUser != null)
        {
            loginAsLastUsedUserButton.SetActive(true);
            loginAsLastUsedUserButton.GetComponentInChildren<TMP_Text>().text = $"Log In as {lastUsedUser.Name}";
        }
    }

    public void SetUpExistingUsersList(List<UserIdNamePair> pairs)
    {
        noExistingUsersNotifications.SetActive(pairs.Count == 0);

        // sort list of pairs by userId
        pairs.Sort((p1, p2) => p1.userId.CompareTo(p2.userId));

        foreach (UserIdNamePair p in pairs)
        {
            GameObject prefab = Instantiate(existingUserScrollViewItemPrefab, existingUserScrollViewContentTransform);
            ExistingUserScrollViewItem item = prefab.GetComponent<ExistingUserScrollViewItem>();
            item.SetUpScrollViewItem(p);
        }
    }

    public void ShowDeletionPopUp(int userId, string userName)
    {
        idOfUserToDelete = userId;
        deletionText.text = $"Delete {userName} (ID: {userId})?";
        deletionPopUp.Open();
    }

    public void CancelDeletion()
    {
        idOfUserToDelete = -1;
        deletionPopUp.Close();
    }

    public void ConfirmDeletion()
    {
        // remove list item from list of existing users 
        foreach (Transform item in existingUserScrollViewContentTransform)
        {
            if (item.gameObject.GetComponent<ExistingUserScrollViewItem>().userId == idOfUserToDelete)
            {
                Destroy(item.gameObject);
                break;
            }
        }

        //if you delete user from last session, hide button to login as user from last session 
        if (PlayerPrefs.HasKey("user") && idOfUserToDelete == PlayerPrefs.GetInt("user"))
        {           
            loginAsLastUsedUserButton.SetActive(false);
        }

        loginManager.DeleteExistingUser(idOfUserToDelete);

        idOfUserToDelete = -1;
        deletionPopUp.Close();
    }

    public void OnLoadExistingUserButtonClicked()
    {
        loginUI.SetActive(false);
        loadUserUI.SetActive(true);
    }

    public void OnCreateNewUserButtonClicked()
    {
        userCreationManager.SetUpUserCreation();

        loginUI.SetActive(false);
        createUserUI.SetActive(true);
    }

    public void ShowHomeMenu()
    {
        loginUI.SetActive(true);
        loadUserUI.SetActive(false);
        createUserUI.SetActive(false);
    }
}
