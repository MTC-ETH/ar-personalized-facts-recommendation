using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserCreationManager : MonoBehaviour
{
    [SerializeField] private LoginManager loginManager;
    [SerializeField] private LoginUIManager loginUiManager;
    [Space]
    [SerializeField] private FieldsOfInterestToggles fieldsOfInterestToggles;
    [Space]
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private List<GameObject> continueButtons;
    [Space]
    [SerializeField] private List<PersonalityTraitToggleGroup> personalityTraitToggleGroups;
    [Space]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField ageInputField;
    [SerializeField] private Toggle maleToggle, femaleToggle;

    private int currentPageIndex;

    private void Update()
    {
        // only show "Continue" buttons if everything is filled out
        if (currentPageIndex == 0)
        {
            continueButtons[0].SetActive(FirstPageFilledOut());
        }
        else
        {
            continueButtons[currentPageIndex].SetActive(personalityTraitToggleGroups[currentPageIndex - 1].FilledOut()); // offset of -1 because personality questions start on page 1
        }
    }

    private bool FirstPageFilledOut()
    {
        return nameInputField.text.Length > 0 && ageInputField.text.Length > 0 && (maleToggle.isOn || femaleToggle.isOn);
    }

    public void SetUpUserCreation()
    {
        currentPageIndex = 0;
        for(int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == 0); // disable all pages but the first one
        }

        nameInputField.text = "";
        ageInputField.text = "";
        maleToggle.isOn = false;
        femaleToggle.isOn = false;

        fieldsOfInterestToggles.ResetToggles();

        foreach (PersonalityTraitToggleGroup toggleGroup in personalityTraitToggleGroups)
        {
            toggleGroup.ResetToggles();
        }
    }

    public async void FinishUserCreation()
    {
        UserItem user = new UserItem()
        {
            UserId = await loginManager.GetMaxUserId() + 1,
            Name = nameInputField.text,
            Age = int.Parse(ageInputField.text),
            Gender = maleToggle.isOn ? "Male" : "Female",
            FieldsOfInterest = fieldsOfInterestToggles.GetFieldsOfInterestValues(),
            PersonalityTraits = GetPersonalityTraitValues()
        };

        loginManager.SaveNewUser(user);
    }

    private Dictionary<string, int> GetPersonalityTraitValues()
    {
        Dictionary<string, int> values = new Dictionary<string, int>();

        foreach (PersonalityTraitToggleGroup toggleGroup in personalityTraitToggleGroups)
        {
            values.Add(toggleGroup.personalityTrait, toggleGroup.GetPersonalityTraitValue());
        }

        return values;
    }

    public void OnContinueButtonClicked()
    {
        pages[currentPageIndex].SetActive(false);
        currentPageIndex++;
        pages[currentPageIndex].SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        // return to home menu if on first page
        if (currentPageIndex == 0)
        {
            loginUiManager.ShowHomeMenu();
        }
        else
        {
            pages[currentPageIndex].SetActive(false);
            currentPageIndex--;
            pages[currentPageIndex].SetActive(true);
        }
    }
}
