using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

class InternalDayDefinition
{
    public string Directory;
    public string Date;
    public List<UserPoolDefinition> PoolDefinitions;
    public List<UserPool> PoolOrder;

    private int currentOrder;
    private int currentCountInOrder;

    public InternalDayDefinition(DayDefinition day)
    {
        Directory = day.Directory;
        Date = day.Date;
        PoolDefinitions = new List<UserPoolDefinition>();
        PoolOrder = new List<UserPool>();

        currentOrder = 0;
        currentCountInOrder = 0;

        // Copy pools
        foreach (UserPoolDefinition def in day.PoolDefinitions)
        {
            PoolDefinitions.Add(def.Clone());
        }
        foreach (UserPool pool in day.PoolOrder)
        {
            PoolOrder.Add(pool.Clone());
        }

        if (PoolOrder.Count != 0)
        {
            foreach (UnitGameEvent before in PoolOrder[0].Before)
            {
                before.Emit();
            }
        }
    }

    public string PopNextUser()
    {
        // If we try to access an order past how many we have defined
        if (currentOrder >= PoolOrder.Count)
        {
            return null;
        }

        // If we finish the current order
        if (++currentCountInOrder > PoolOrder[currentOrder].UserCount)
        {
            // Call all after events
            foreach (UnitGameEvent after in PoolOrder[currentOrder].After)
            {
                after.Emit();
            }

            // Increment the order and reset the order counter
            currentCountInOrder = 0;
            currentOrder += 1;

            // Check again for an access of an order past the defined count
            if (currentOrder >= PoolOrder.Count)
            {
                return null;
            }

            // Call all before events on the new order
            foreach (UnitGameEvent before in PoolOrder[currentOrder].Before)
            {
                before.Emit();
            }
        }

        // If the selected pool definition doesn't exist
        if (PoolOrder[currentOrder].PoolIndex > PoolDefinitions.Count)
        {
            return null;
        }

        UserPoolDefinition pool = PoolDefinitions[PoolOrder[currentOrder].PoolIndex];

        // If the selected pool definition has no remaining users
        if (pool.UserFiles.Count == 0)
        {
            return null;
        }

        // Pick a random index
        System.Random rand = new System.Random();
        int idx = rand.Next(pool.UserFiles.Count);

        string file = pool.UserFiles[idx];
        pool.UserFiles.RemoveAt(idx);

        return file;
    }
}

public class UserManager : Subscriber
{
    public DayDefinition Day;
    private InternalDayDefinition day;

    private UserEntry? currentUser;

    private Dictionary<string, UserEntry> users;

    [Header("Event Listeners")]
    public BoolGameEvent ResolveAppeal;
    public UnitGameEvent UserInfoRequest;

    [Header("Events")]
    public UserEntryGameEvent UserLoaded;

    // `true` implies player chose correctly, `false` implies player chose incorrectly
    public BoolGameEvent AfterAppeal;
    public UnitGameEvent DayFinished; //For EOD
    private bool dayAlreadyFinished = false;

    protected override void Subscribe()
    {
        ResolveAppeal?.Subscribe(OnResolveAppeal);
        UserInfoRequest?.Subscribe(OnUserInfoRequest);
    }

    protected override void AfterSubscribe()
    {
        if (Day == null)
        {
            Day = new DayDefinition();
        }

        day = new InternalDayDefinition(Day);

        // load users
        users = JSONImporter.ImportDirectory<UserEntry>(
            Path.Combine("lang", "en", "days", day.Directory)
        );

        // (string ruleName, string checking, string ruleType, var criteria)

        currentUser = null;
        MoveToNextUser();
    }

    // moves to next user index
    private bool MoveToNextUser()
    {
        // if we somehow get here with no users
        if (users == null || users.Count == 0)
        {
            currentUser = null;
            SignalDayFinishedOnce();
            return false;
        }

        string user_file = day.PopNextUser();
        if (user_file != null && users.ContainsKey(user_file))
        {
            currentUser = users[user_file];
        }
        else
        {
            currentUser = null;
            SignalDayFinishedOnce();
            return false;
        }

        OnUserInfoRequest();

        // return true if successful
        return true;
    }

    private void SignalDayFinishedOnce()
    {
        if (dayAlreadyFinished)
            return;
        dayAlreadyFinished = true;
        DayFinished?.Emit();
        UserLoaded?.Emit(new UserEntry());
    }

    private void OnResolveAppeal(bool decision)
    {
        UserEntry? user = currentUser;
        if (user != null)
        {
            // bool success = decision == user.Value.should_approve;
            bool success = true;
            int numMsg=0;
            
            // msg len > 50 char
            // foreach (string message in user.messages)
            // {
            //     Debug.Log("There is no god");
            // }

            // mesg len <=50
            foreach (string message in user.Value.messages)
            {
                if (message.Length> 50){success = false;}
                numMsg++;
                if(Regex.IsMatch(message.ToLower(),@".*cat.*")){success=false;}
                if(Regex.IsMatch(message.ToLower(),@".*(.)\1{2,}.*")){success=false;}
            }

            if(user.Value.appeal_message == ""){success=false;}
            if(user.Value.name.Length > 12){success=false;}
            if(user.Value.bio.Split(' ').Length<4){success=false;}

            AfterAppeal?.Emit(success);
        }

        MoveToNextUser();
    }

    private void OnUserInfoRequest()
    {
        UserEntry? user = currentUser;
        if (user != null)
        {
            UserLoaded?.Emit(user.Value);
        }
    }
}
