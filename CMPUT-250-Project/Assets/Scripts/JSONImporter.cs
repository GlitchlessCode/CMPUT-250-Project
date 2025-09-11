using System.Collections.Generic;
using System.IO;
using UnityEngine;

// For now, this dataclass will just be kept inside of this file too
[System.Serializable]
public class UserEntry
{
    // Props are readonly
    public string name;

    public string[] messages;
}

public class JSONImporter : MonoBehaviour
{
    void Start()
    {
        // Example using ImportDirectory
        List<UserEntry> users = ImportDirectory<UserEntry>(Path.Combine("lang", "en", "users"));
        foreach (UserEntry user in users)
        {
            Debug.Log("name: " + user.name + ", messages: " + string.Join(",", user.messages));
        }
    }

    /// <summary>
    /// Import all .json files in a directory as type T
    /// </summary>
    public List<T> ImportDirectory<T>(string dir_path)
    {
        string path = Path.Combine(Application.streamingAssetsPath, dir_path);

        // All try-catches are commented for now to imply fallability, even if I haven't taken the time to implement correct error handling
        // try
        // {
        List<T> items = new List<T>();
        foreach (string filename in Directory.EnumerateFiles(path))
        {
            if (filename.EndsWith(".json"))
            {
                items.Add(ImportFile<T>(filename));
            }
        }
        return items;
        // }
        // catch (Exception err)
        // {
        // }
    }

    private T ImportFile<T>(string filename)
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

    private string ReadFileToString(string path)
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
