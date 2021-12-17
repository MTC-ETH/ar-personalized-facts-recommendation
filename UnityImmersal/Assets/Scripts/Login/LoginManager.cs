using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateClean;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private LoginUIManager loginUIManager;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private AwsUserManager awsUserManager;

    private async void Start()
    {
        if (PlayerPrefs.HasKey("user"))
        {
            int userId = PlayerPrefs.GetInt("user");
            UserItem lastUsedUser = await awsUserManager.LoadUser(userId);

            loginUIManager.SetUpLoginAsLastUsedUserButton(lastUsedUser);
        }

        List<UserIdNamePair> userIdNamePairs = await awsUserManager.LoadAllUserNamesAndIds();

        loginUIManager.SetUpExistingUsersList(userIdNamePairs);
    }

    public void DeleteExistingUser(int userId)
    {
        // if you delete user from last session, delete entry on device
        if (PlayerPrefs.HasKey("user") && userId == PlayerPrefs.GetInt("user"))
        {
            PlayerPrefs.DeleteKey("user");
            PlayerPrefs.Save();
        }

        awsUserManager.DeleteUser(userId);
    }

    // Called after selecting an user from the list of existing users
    public void LoadExistingUser(int userId)
    {
        PlayerPrefs.SetInt("user", userId);
        PlayerPrefs.Save();
        LoadMainScene();
    }

    // called after creating a new user profile
    public void SaveNewUser(UserItem newUser)
    {
        awsUserManager.SaveUser(newUser);
        PlayerPrefs.SetInt("user", newUser.UserId);
        PlayerPrefs.Save();
        LoadMainScene();
    }

    public void LoadMainScene()
    {
        sceneTransition.PerformTransition();
    }


    public async Task<int> GetMaxUserId()
    {
        List<UserIdNamePair> pairs = await awsUserManager.LoadAllUserNamesAndIds();

        int maxId = -1; 
        foreach (UserIdNamePair p in pairs)
        {
            if (p.userId > maxId)
            {
                maxId = p.userId;
            }
        }

        return maxId;
    }
}
