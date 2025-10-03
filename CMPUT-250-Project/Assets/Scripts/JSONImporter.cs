using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public struct UserEntry
{
    public string name;
    public string date;
    public string bio;
    public int image_index;
    public string[] messages;
    public string appeal_message;
    public bool should_approve;
}

[System.Serializable]
public struct DirectMessage { }

public static class JSONImporter
{
    private static Regex regex = new Regex(@"([^\/\\]+)(?:\.json)$");

    /// <summary>
    /// Import all .json files in a directory as type T
    /// </summary>
    public static Dictionary<string, T> ImportDirectory<T>(string dir_path)
    {
        string path = Path.Combine(Application.streamingAssetsPath, dir_path);

        // All try-catches are commented for now to imply fallability, even if I haven't taken the time to implement correct error handling
        // try
        // {
        Dictionary<string, T> items = new Dictionary<string, T>();

        foreach (string filename in Directory.EnumerateFiles(path))
        {
            if (filename.EndsWith(".json"))
            {
                string simple_name = regex.Match(filename).Groups[1].Value;
                items.Add(simple_name, ImportFile<T>(filename));
            }
        }
        return items;
        // }
        // catch (Exception err)
        // {
        // }
    }

    private static T ImportFile<T>(string filename)
    {
        // try
        // {
        string file_contents = ReadFileToString(filename);

        T type = JsonUtility.FromJson<T>(file_contents);
        return type;
        // }
        // catch (Exception err)
        // {
        // }
    }

    private static string ReadFileToString(string path)
    {
        // try
        // {
        // Ensures the reader is disposed of automatically at the end of scope.
        using (StreamReader reader = new StreamReader(path))
        {
            string contents = reader.ReadToEnd();
            return contents;
        }
        // }
        // catch (Exception err)
        // {
        // }
    }
}
