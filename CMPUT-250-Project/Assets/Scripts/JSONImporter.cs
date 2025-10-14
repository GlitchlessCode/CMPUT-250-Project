using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct UserEntry
{
    public string name;
    public string date;
    public string bio;
    public int image_index;
    public string[] messages;
    public string appeal_message;
}

[System.Serializable]
public struct DirectMessage
{
    public string message;
}

public static class JSONImporter
{
    // Import all requested .json files as type T
    //
    // All filenames should be extension-less, .json is assumed
    public static System.Collections.IEnumerator ImportFiles<T>(
        string dir_path,
        IEnumerable<string> files,
        Action<Dictionary<string, T>> callback
    )
    {
        string path = Path.Combine(Application.streamingAssetsPath, dir_path);

        Dictionary<string, T> items = new Dictionary<string, T>();

        foreach (string filename in files)
        {
            T loaded_content = default(T);
            yield return ImportFile<T>(
                Path.ChangeExtension(Path.Combine(path, filename), "json"),
                (content) =>
                {
                    loaded_content = content;
                }
            );
            items.Add(filename, loaded_content);
        }

        callback.Invoke(items);
    }

    private static System.Collections.IEnumerator ImportFile<T>(string filename, Action<T> callback)
    {
        yield return ReadFileToString(
            filename,
            (file_contents) =>
            {
                callback(JsonUtility.FromJson<T>(file_contents));
            }
        );
    }

    private static System.Collections.IEnumerator ReadFileToString(
        string path,
        Action<string> callback
    )
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();
            if (!request.isNetworkError)
            {
                callback(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to load file, UnityWebRequest returned network error");
                callback("{}");
            }
        }
#else
        using (StreamReader reader = new StreamReader(path))
        {
            callback(reader.ReadToEnd());
        }
        yield return null;
#endif
    }
}
