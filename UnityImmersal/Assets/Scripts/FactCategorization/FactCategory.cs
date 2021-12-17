using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FactCategorization
{
    public class FactCategory : MonoBehaviour
    {
        [SerializeField] private Toggle toggle0, toggle1, toggle2;
        [SerializeField] private TMP_Text categoryNameText;

        public bool AllValuesSet()
        {
            // Toggle group components ensure that at most one toggle is on ==> only need to check if at least one toggle is on 
            return toggle0.isOn || toggle1.isOn || toggle2.isOn;
        }

        public void SetCategoryName(string categoryName)
        {
            categoryNameText.text = categoryName;
        }

        public string GetCategoryName()
        {
            return categoryNameText.text;
        }

        public int GetCategoryValue()
        {
            return toggle0.isOn ? 0 : (toggle1.isOn ? 1 : 2);
        }
    }
}