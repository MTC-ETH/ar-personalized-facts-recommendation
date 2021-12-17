using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalityTraitToggleGroup : MonoBehaviour
{
    public string personalityTrait;

    [SerializeField] private Toggle toggle1, toggle2, toggle3, toggle4, toggle5, toggle6, toggle7;

    public int GetPersonalityTraitValue()
    {
        return toggle1.isOn ? 1 : (toggle2.isOn ? 2 : (toggle3.isOn ? 3 : (toggle4.isOn ? 4 : (toggle5.isOn ? 5 : (toggle6.isOn ? 6 : 7)))));
    }

    public void ResetToggles()
    {
        toggle1.isOn = false;
        toggle2.isOn = false;
        toggle3.isOn = false;
        toggle4.isOn = false;
        toggle5.isOn = false;
        toggle6.isOn = false;
        toggle7.isOn = false;
    }

    public bool FilledOut()
    {
        return toggle1.isOn || toggle2.isOn || toggle3.isOn || toggle4.isOn || toggle5.isOn || toggle6.isOn || toggle7.isOn;
    }
}
