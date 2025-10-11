using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Direct Messages/Sequence")]
public class DirectMessageSequenceDefinition : ScriptableObject
{
    public string directory;
    public List<string> dmFiles;

    public List<DirectMessage> GetMessages()
    {
        try
        {
            System.Random rand = new System.Random();

            Dictionary<string, DirectMessage> files = JSONImporter.ImportDirectory<DirectMessage>(
                Path.Combine("lang", "en", "messages", directory)
            );

            List<DirectMessage> messages = new List<DirectMessage>();
            foreach (string filename in dmFiles)
            {
                messages.Add(files[filename]);
            }

            return messages;
        }
        catch (Exception err)
        {
            Debug.LogWarning(
                "Failed to load DirectMessageSequence " + directory + " due to " + err
            );
            return new List<DirectMessage>();
        }
    }
}
