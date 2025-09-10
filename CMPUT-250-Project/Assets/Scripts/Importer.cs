using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For now, this dataclass will just be kept inside of this file too
public class UserEntry
{
    // Props are readonly
    [field: SerializeField] public string Name {get; private set;}
    [field: SerializeField] public string[] Messages {get; private set;}

    public UserEntry(string name, string[] messages) {
        Name = name;
        Messages = messages;
    }
}

public class Importer : MonoBehaviour
{
    void Start() {
        List<UserEntry> users = ImportAllUsers();       
        foreach (UserEntry user in users) {
            Debug.Log("name: " + user.Name + ", messages: " + string.Join(",", user.Messages));
        }
    }
    
    List<UserEntry> ImportAllUsers() {
        List<UserEntry> users = new List<UserEntry>();
        users.Add(new UserEntry("sample_user", new string[] {"a message", "another message"}));
        // UserEntry user = JsonUtility.FromJson<UserEntry>("{\"name\"}")
        return users;
    }
}


