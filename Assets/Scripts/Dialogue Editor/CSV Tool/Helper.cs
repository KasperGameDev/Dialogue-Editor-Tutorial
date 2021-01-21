using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Helper 
{
    public static List<T> FindAllObjectFromResources<T>()
    {
        List<T> tmp = new List<T>();
        string ResourcesPath = Application.dataPath + "/Resources";
        string[] directories = Directory.GetDirectories(ResourcesPath,"*",SearchOption.AllDirectories);

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
}
