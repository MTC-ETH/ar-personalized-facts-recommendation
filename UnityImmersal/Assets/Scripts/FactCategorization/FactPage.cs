using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace FactCategorization
{
    public class FactPage : MonoBehaviour
    {
        [SerializeField] private TMP_Text factTextField;
        [Space]
        [SerializeField] private Transform factCategoriesParent;
        [SerializeField] private GameObject factCategoryPrefab;

        private List<FactCategory> factCategories;

        private void Start()
        {
            List<string> categories = GameObject.Find("CategorizationManager").GetComponent<FactCategorizationManager>().categories;

            factCategories = new List<FactCategory>();
            foreach (string category in categories)
            {
                GameObject prefab = Instantiate(factCategoryPrefab, factCategoriesParent);
                FactCategory factCategory = prefab.GetComponent<FactCategory>();
                factCategory.SetCategoryName(category);
                factCategories.Add(factCategory);
            }
        }

        public void SetFact(string fact)
        {
            factTextField.text = fact;
        }

        public bool AllValuesSet()
        {
            if (factCategories == null) return false;

            foreach (FactCategory category in factCategories)
            {
                if (!category.AllValuesSet())
                {
                    return false;
                }
            }

            return true;
        }

        public string GetFact()
        {
            return factTextField.text;
        }

        public Dictionary<string, int> GetCategoryValues()
        {
            Dictionary<string, int> categoryValues = new Dictionary<string, int>();

            foreach (FactCategory factCategory in factCategories)
            {
                categoryValues.Add(factCategory.GetCategoryName(), factCategory.GetCategoryValue());
            }

            return categoryValues;
        }
    }
}