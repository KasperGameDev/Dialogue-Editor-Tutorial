using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DialogueEditor.Dialogue.Editor
{
    public static class Helper
    {
        // Old way of find Dialogue Containers.
        // This work with generic.
        // may work in run time need to check.
        // TODO: check if it work in runtime at some point.
        public static List<T> FindAllObjectFromResources<T>()
        {
            List<T> tmp = new List<T>();
            string ResourcesPath = Application.dataPath + "/Resources";
            string[] directories = Directory.GetDirectories(ResourcesPath, "*", SearchOption.AllDirectories);

            foreach (string directorie in directories)
            {
                string directoriePath = directorie.Substring(ResourcesPath.Length + 1);
                T[] reault = Resources.LoadAll(directoriePath, typeof(T)).Cast<T>().ToArray();

                foreach (T item in reault)
                {
                    if (!tmp.Contains(item))
                    {
                        tmp.Add(item);
                    }
                }
            }

            return tmp;
        }

        /// <summary>
        /// Find all Dialogue Containers in Assets
        /// </summary>
        /// <returns>List of Dialogue Containers</returns>
        public static List<DialogueContainerSO> FindAllDialogueContainerSO()
        {
            // Find all the DialogueContainerSO in Assets and get it guid ID.
            string[] guids = AssetDatabase.FindAssets("t:DialogueContainerSO");

            // Make a Array as long as we found DialogueContainerSO.
            DialogueContainerSO[] items = new DialogueContainerSO[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);                  // Use the guid ID to find the Asset path. 
                items[i] = AssetDatabase.LoadAssetAtPath<DialogueContainerSO>(path);    // Use path to find and load DialogueContainerSO.
            }

            return items.ToList();
        }
    }
}