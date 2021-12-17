using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FactCategorization
{
    public class FactCategorizationUIManager : MonoBehaviour
    {
        [SerializeField] private FactCategorizationManager factCategorizationManager;
        [Space]
        [SerializeField] private GameObject homeUI;
        [SerializeField] private GameObject categorizationUI;
        [Space]
        [SerializeField] private Transform factPagesParent;
        [SerializeField] private GameObject factPagePrefab;
        [Space]
        [SerializeField] private GameObject nextPageButton;
        [SerializeField] private GameObject previousPageButton;
        [Space]
        [SerializeField] private GameObject finalizationPage;
        [SerializeField] private TMP_InputField poiIdInputField;
        [SerializeField] private TMP_InputField poiNameInputField;
        [SerializeField] private GameObject doneButton;

        private List<FactPage> pages = new List<FactPage>();
        private int currentPageIndex;

        private void Awake()
        {
            ShowHomeMenu();
        }

        private void Update()
        {
            if (pages.Count == 0)
                return;

            // Update visibility of buttons

            if (currentPageIndex < pages.Count)
            {
                nextPageButton.SetActive(pages[currentPageIndex].AllValuesSet());
            }
            else
            {
                nextPageButton.SetActive(false);
            }

            previousPageButton.SetActive(currentPageIndex > 0);

            doneButton.SetActive(poiIdInputField.text.Length > 0 && poiNameInputField.text.Length > 0);
        }

        public void ShowHomeMenu()
        {
            homeUI.SetActive(true);
            categorizationUI.SetActive(false);
        }

        public void SetUpCategorizationUI(List<string> facts)
        {
            homeUI.SetActive(false);
            categorizationUI.SetActive(true);

            finalizationPage.SetActive(false);

            // clear input fields
            poiIdInputField.text = "";
            poiNameInputField.text = "";

            // Delete page GameObjects
            foreach (FactPage page in pages)
            {
                Destroy(page.gameObject);
            }

            // Reset list of pages
            pages.Clear();
            currentPageIndex = 0;

            StartCoroutine(CreateFactPages(facts));
        }

        private IEnumerator CreateFactPages(List<string> facts)
        {
            // Spread creation process across multiple frames
            for (int i = 0; i < facts.Count; i++)
            {
                GameObject go = Instantiate(factPagePrefab, factPagesParent);

                FactPage page = go.GetComponent<FactPage>();
                page.SetFact(facts[i]);
                pages.Add(page);

                //disable all pages but the first one
                if (i > 0)
                {
                    go.SetActive(false);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public void OnDoneButtonClicked()
        {
            List<PoiFactItem> poiFactItems = new List<PoiFactItem>();

            foreach (FactPage page in pages)
            {
                PoiFactItem item = new PoiFactItem()
                {
                    PoiId = int.Parse(poiIdInputField.text),
                    FactId = pages.IndexOf(page),
                    PoiName = poiNameInputField.text,

                    Fact = page.GetFact(),
                    Categories = page.GetCategoryValues(),
                };

                poiFactItems.Add(item);
            }

            factCategorizationManager.SaveFactsToDatabase(poiFactItems);

            ShowHomeMenu();
        }

        public void GoToNextPage()
        {
            pages[currentPageIndex].gameObject.SetActive(false);

            currentPageIndex++;

            if (currentPageIndex == pages.Count)
            {
                finalizationPage.SetActive(true);
            }
            else
            {
                pages[currentPageIndex].gameObject.SetActive(true);
            }
        }

        public void GoToPreviousPage()
        {
            finalizationPage.SetActive(false);

            if (currentPageIndex < pages.Count)
            {
                pages[currentPageIndex].gameObject.SetActive(false);
            }

            currentPageIndex--;

            pages[currentPageIndex].gameObject.SetActive(true);
        }
    }
}
