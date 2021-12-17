using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;

namespace FactCategorization
{
    public class FactCategorizationManager : MonoBehaviour
    {
        [SerializeField] private FactCategorizationUIManager uiManager;
        [SerializeField] private AwsPoiFactManager awsPoiFactManager;
        [Space]
        public List<string> categories;

        public void OpenTextFile()
        {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "txt", false);

            string path = paths[0]; //multiselect disbled ==> only one path in array

            if (path.Length == 0)
                return;


            List<string> facts = new List<string>();

            string line;
            StreamReader sr = new StreamReader(path);
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Length > 0)
                {
                    facts.Add(line);
                }
            }
            sr.Close();

            uiManager.SetUpCategorizationUI(facts);
        }

        public void SaveFactsToDatabase(List<PoiFactItem> items)
        {
            foreach (PoiFactItem item in items)
            {
                awsPoiFactManager.SavePoiFact(item);
            }
        }
    }
}
