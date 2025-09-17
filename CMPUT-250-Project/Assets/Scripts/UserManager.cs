using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    private JSONImporter jsonImporter;
    private int currentUserIndex = 0;
    private List<UserEntry> users;

    void Awake()
    {
        // load users
        jsonImporter = GetComponent<JSONImporter>();
        users = jsonImporter.ImportDirectory<UserEntry>(Path.Combine("lang", "en", "days", "day1"));

        // set first user
        if (users == null || users.Count == 0)
        {
            currentUserIndex = -1; // no users loaded
        }
        else
        {
            currentUserIndex = 0; // first user by default
        }
    }

    void Update()
    {
        // what i commented out is an example usage of the methods in usermanager

        // // testing
        // if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
        //     MoveToNextUser();
        // }
        // if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
        //     Debug.Log(GetCurrentUserName());
        // }
    }

    public UserEntry? GetCurrentUser()
    {
        if (users != null && currentUserIndex >= 0 && currentUserIndex < users.Count)
        {
            return users[currentUserIndex];
        }

        // Might be worth making an "error" user that just displays a bunch of shit to use when things go wrong
        // for now im assuming you wont call this if you dont have a user
        // later problem
        return null;
    }

    // moves to next user index
    public bool MoveToNextUser()
    {
        // if we somehow get here with no users
        if (users == null || users.Count == 0)
        {
            return false;
        }

        // for now, loop back to first user after last one
        currentUserIndex++;

        if (currentUserIndex >= users.Count)
        {
            currentUserIndex = 0;
        }

        // return true if successful
        return true;
    }

    // sets index and returns true if it worked
    public bool SetCurrentUser(int userIndex)
    {
        if (currentUserIndex >= users.Count)
        {
            currentUserIndex = userIndex;
            return true;
        }

        return false;
    }

    // get users username
    public string GetCurrentUserName()
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.name;
        }
        else
        {
            return null;
        }
    }

    // get the date for your messages
    public string GetCurrentUserDate()
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.date;
        }
        else
        {
            return null;
        }
    }

    // get your users bio
    public string GetCurrentUserBio()
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.bio;
        }
        else
        {
            return null;
        }
    }

    // get the index related to what the current image should be
    public int? GetCurrentUserImg()
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.image_index;
        }
        else
        {
            return null;
        }
    }

    // get the appeal message
    public string GetCurrentUserAppeal()
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.appeal_message;
        }
        else
        {
            return null;
        }
    }

    // get the approval bool
    public bool? GetCurrentUserApproval()
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.should_approve;
        }
        else
        {
            return null;
        }
    }

    // get a specific message
    public string GetCurrentUserMessage(int index)
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.messages[index];
        }
        else
        {
            return null;
        }
    }

    // get all messages
    public string[] GetCurrentUserMessagesAll()
    {
        var user = GetCurrentUser();

        if (user != null)
        {
            return user.Value.messages;
        }
        else
        {
            return null;
        }
    }

    // will make a method to generate random times later - for prototype we can just use dates
}
