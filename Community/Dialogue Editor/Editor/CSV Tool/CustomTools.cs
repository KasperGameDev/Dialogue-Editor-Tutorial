using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogueEditor.Dialogue.Editor
{
    public class CustomTools
    {
        [MenuItem("Tools/Dialogue Editor/Save to CSV")]
        public static void SaveToCSV()
        {
            SaveCSV saveCSV = new SaveCSV();
            saveCSV.Save();

            EditorApplication.Beep();
            Debug.Log("<color=green> Save CSV File successfully! </color>");
        }

        [MenuItem("Tools/Dialogue Editor/Load from CSV")]
        public static void LoadFromCSV()
        {
            LoadCSV loadCSV = new LoadCSV();
            loadCSV.Load();

            EditorApplication.Beep();
            Debug.Log("<color=green> Loading CSV File successfully! </color>");
        }
    }
}