using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KasperDev.Dialogue.Editor
{
    public class SaveCSV
    {
        private string csvDirectoryName = "Resources/Dialogue Editor/CSV File";
        private string csvFileName = "DialogueCSV_Save.csv";
        private string csvSeparator = ",";
        private List<string> csvHeader;
        private string node_ID = "Node Guid ID";
        private string text_ID = "Text Guid ID";
        private string dialogueName = "Dialogue Name";

        public void Save()
        {
            List<DialogueContainerSO> dialogueContainers = Helper.FindAllDialogueContainerSO();

            CreateFile();

            foreach (DialogueContainerSO dialogueContainer in dialogueContainers)
            {
                foreach (DialogueData nodeData in dialogueContainer.DialogueDatas)
                {
                    foreach (DialogueData_Text textData in nodeData.DialogueData_Texts)
                    {
                        List<string> texts = new List<string>();

                        texts.Add(dialogueContainer.name);
                        texts.Add(nodeData.NodeGuid);
                        texts.Add(textData.GuidID.Value);

                        foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
                        {
                            string tmp = textData.Text.Find(language => language.LanguageType == languageType).LanguageGenericType.Replace("\"", "\"\"");
                            texts.Add($"\"{tmp}\"");
                        }

                        AppendToFile(texts);
                    }
                }

                foreach (ChoiceData nodeData in dialogueContainer.ChoiceDatas)
                {
                    List<string> texts = new List<string>();

                    texts.Add(dialogueContainer.name);
                    texts.Add(nodeData.NodeGuid);
                    texts.Add("Choice Dont have Text ID");

                    foreach (LanguageType languageType in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
                    {
                        string tmp = nodeData.Text.Find(language => language.LanguageType == languageType).LanguageGenericType.Replace("\"", "\"\"");
                        texts.Add($"\"{tmp}\"");
                    }

                    AppendToFile(texts);
                }
            }
        }

        private void AppendToFile(List<string> strings)
        {
            using (StreamWriter sw = File.AppendText(GetFilePath()))
            {
                string finalString = "";
                foreach (string text in strings)
                {
                    if (finalString != "")
                    {
                        finalString += csvSeparator;
                    }
                    finalString += text;
                }

                sw.WriteLine(finalString);
            }
        }

        private void CreateFile()
        {
            VerifDirectory();
            MakeHeader();
            using (StreamWriter sw = File.CreateText(GetFilePath()))
            {
                string finalString = "";
                foreach (string header in csvHeader)
                {
                    if (finalString != "")
                    {
                        finalString += csvSeparator;
                    }
                    finalString += header;
                }

                sw.WriteLine(finalString);
            }
        }

        private void MakeHeader()
        {
            List<string> headerText = new List<string>();
            headerText.Add(dialogueName);
            headerText.Add(node_ID);
            headerText.Add(text_ID);

            foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
            {
                headerText.Add(language.ToString());
            }

            csvHeader = headerText;
        }

        private void VerifDirectory()
        {
            string directory = GetDirectoryPath();

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string GetDirectoryPath()
        {
            return $"{Application.dataPath}/{csvDirectoryName}";
        }

        private string GetFilePath()
        {
            return $"{GetDirectoryPath()}/{csvFileName}";
        }
    }
}