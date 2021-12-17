using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateClean;

public class UserSettingsPopup : MonoBehaviour
{
    [SerializeField] private CustomPopup popup;
    [SerializeField] private FieldsOfInterestToggles toggles;
    [SerializeField] private AwsUserManager awsUserManager;
    [SerializeField] private ImmersalManager immersalManager;
    [SerializeField] private RecommendationCommunication recommendationCommunication;
    [SerializeField] private Switch personalizationSwitch;

    private UserItem currentUser;

    private bool switchOn;

    private void Awake()
    {
        popup.gameObject.SetActive(true);

        switchOn = true;
    }

    public async void OnSettingsButtonClicked()
    {
        popup.Open();

        switchOn = recommendationCommunication.retrievePersonalizedFacts;

        currentUser = await awsUserManager.LoadUser(PlayerPrefs.GetInt("user"));
        toggles.LoadUserPreferences(currentUser);
    }

    public void OnSaveButtonClicked()
    {
        popup.Close();

        recommendationCommunication.retrievePersonalizedFacts = switchOn;

        currentUser.FieldsOfInterest = toggles.GetFieldsOfInterestValues();
        awsUserManager.SaveUser(currentUser);

        // Reset AR Maps and POI content such that they can be retrieved again with new preferences or with/without personalization
        immersalManager.ResetARMapsAndPoiContent();
    }

    public void OnCloseButtonClicked()
    {
        // revert switch
        if (switchOn != recommendationCommunication.retrievePersonalizedFacts)
        {
            personalizationSwitch.Toggle();
        }

        popup.Close();
    }

    public void OnPersonalizationSwitched()
    {
        switchOn = !switchOn;
    }
}
