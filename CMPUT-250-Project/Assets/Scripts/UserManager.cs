using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

class InternalDayDefinition
{
    public int Index;
    public string Directory;
    public string Date;
    public List<UserPoolDefinition> PoolDefinitions;
    public List<UserPool> PoolOrder;

    private int currentOrder;
    private int currentCountInOrder;

    public HashSet<string> UserFiles
    {
        get
        {
            HashSet<string> files = new HashSet<string>();

            foreach (UserPoolDefinition pool in PoolDefinitions)
            {
                pool.UserFiles.ForEach((item) => files.Add(item));
            }

            return files;
        }
    }

    public InternalDayDefinition(DayDefinition day)
    {
        Index = day.Index;
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
    Validator validator = new Validator();

    private Dictionary<string, UserEntry> users;

    [Header("Event Listeners")]
    public BoolGameEvent ResolveAppeal;
    public UnitGameEvent UserInfoRequest;

    [Header("Events")]
    public UserEntryGameEvent UserLoaded;
    public StringGameEvent ValidatorLoaded;
    public StringGameEvent DayDate;
    public UnitGameEvent AsyncComplete;

    // `true` implies player chose correctly, `false` implies player chose incorrectly
    public BoolGameEvent AfterAppeal;
    public IntGameEvent DayStart;
    public UnitGameEvent DayFinished;
    private bool dayAlreadyFinished = false;

    public override void Subscribe()
    {
        ResolveAppeal?.Subscribe(OnResolveAppeal);
        UserInfoRequest?.Subscribe(OnUserInfoRequest);
    }

    public override void AfterSubscribe()
    {
        addRules();

        if (Day == null)
        {
            Day = new DayDefinition();
        }

        day = new InternalDayDefinition(Day);

        sendDayData(day);

        // load users
        StartCoroutine(
            JSONImporter.ImportFiles<UserEntry>(
                Path.Combine("lang", "en", "days", day.Directory),
                day.UserFiles,
                (usersOut) =>
                {
                    users = usersOut;
                    MoveToNextUser();
                    DayStart?.Emit(day.Index);
                    AsyncComplete?.Emit();
                }
            )
        );

        // (string ruleName, string checking, string ruleType, var criteria)

        currentUser = null;
    }

    private void sendDayData(InternalDayDefinition day)
    {
        DayDate?.Emit(day.Date);
    }

    private void addRules() // examples within
    {
        validator.AddCondition(
            "1. NO swearing allowed!!",
            (currentUser) =>
            {
                return validator.messageRepeatsSpecific(currentUser, 2, @"\*");
            }
        );

        validator.AddCondition(
            "2. Appeal message must exist",
            (currentUser) =>
            {
                return validator.stringLengthCheck(currentUser.Value.appeal_message, ">", 0);
            }
        );

        validator.AddCondition(
            "3. No more than 15 words per message!!",
            (currentUser) =>
            {
                return validator.wordsPerMessage(currentUser, "<=", 15);
            }
        );

        validator.AddCondition(
            "4. Do NOT send messages in ALL CAPITALS!!",
            (currentUser) =>
            {
                return validator.messagesContain(currentUser, "[a-z]");
            }
        );

        validator.AddCondition(
            "5. Plz unban ALL users who have been banned for at least a month!!",
            (currentUser) =>
            {
                return true;
            }
        );

        ValidatorLoaded?.Emit(validator.GetConditionText());
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
        UserEntry empty = new UserEntry();
        empty.messages = new string[0];
        UserLoaded?.Emit(empty);
    }

    private void OnResolveAppeal(bool decision)
    {
        UserEntry? user = currentUser;
        if (user != null)
        {
            AfterAppeal?.Emit(validator.Validate(user, day.Date) == decision);
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
