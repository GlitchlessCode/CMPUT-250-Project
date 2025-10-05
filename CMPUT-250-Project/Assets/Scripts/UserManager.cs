using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserManager : Subscriber
{
    public DayDefinition Day;

    private int currentUserIndex = 0;
    private Dictionary<string, UserEntry> users;

    [Header("Event Listeners")]
    public BoolGameEvent ResolveAppeal;
    public UnitGameEvent UserInfoRequest;

    [Header("Events")]
    public UserEntryGameEvent UserLoaded;

    // `true` implies player chose correctly, `false` implies player chose incorrectly
    public BoolGameEvent AfterAppeal;

    protected override void Subscribe()
    {
        ResolveAppeal?.Subscribe(OnResolveAppeal);
        UserInfoRequest?.Subscribe(OnUserInfoRequest);
    }

    protected override void AfterSubscribe()
    {
        if (Day == null)
        {
            Day = new DayDefinition("daynull");
        }

        // load users
        users = JSONImporter.ImportDirectory<UserEntry>(
            Path.Combine("lang", "en", "days", Day.Directory)
        );

        currentUserIndex = -1;
        MoveToNextUser();
    }

    private UserEntry? GetCurrentUser()
    {
        if (users != null && currentUserIndex >= 0 && currentUserIndex < users.Count)
        {
            return users[Day.Users[currentUserIndex].filename];
        }

        // Might be worth making an "error" user that just displays a bunch of shit to use when things go wrong
        // for now im assuming you wont call this if you dont have a user
        // later problem
        return null;
    }

    // moves to next user index
    private bool MoveToNextUser()
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

        UserLoaded?.Emit(users[Day.Users[currentUserIndex].filename]);

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

    private void OnResolveAppeal(bool decision)
    {
        UserEntry? user = GetCurrentUser();
        if (user != null)
        {
            bool success = decision == user.Value.should_approve;

            AfterAppeal?.Emit(success);
            foreach (BoolGameEvent after in Day.Users[currentUserIndex].after)
            {
                after.Emit(success);
            }
        }

        MoveToNextUser();

        foreach (UnitGameEvent before in Day.Users[currentUserIndex].before)
        {
            before.Emit();
        }
    }

    private void OnUserInfoRequest()
    {
        UserEntry? user = GetCurrentUser();
        if (user != null)
        {
            UserLoaded.Emit(user.Value);
        }
    }

    // will make a method to generate random times later - for prototype we can just use dates
}
