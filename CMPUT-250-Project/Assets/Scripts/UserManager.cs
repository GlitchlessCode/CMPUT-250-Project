using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

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
            currentCountInOrder = 1;
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

    [Header("Validation")]
    private UserEntry? currentUser;
    Validator validator = new Validator();
    public GameObject RedRing;
    public Text BrokenRuleText;
    private Coroutine valRoutine;
    public float MistakeTime = 2f;


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
            "1. NO swearing allowed in chat!",
            (currentUser) =>
            {
                return validator.messageRepeatsSpecific(currentUser, 2, @"\*");
            }
        );

        validator.AddCondition(
            "2. ban appeal MUST exist!",
            (currentUser) =>
            {
                return validator.stringLengthCheck(currentUser.Value.appeal_message, ">", 0);
            }
        );

        validator.AddCondition(
            "3. MAXIMUM 15 words in each chat message!",
            (currentUser) =>
            {
                return validator.wordsPerMessage(currentUser, "<=", 15);
            }
        );

        validator.AddCondition(
            "4. no individual messages sent in ALL CAPS!",
            (currentUser) =>
            {
                return validator.messagesContain(currentUser, @"^[^a-z]*$");
            }
        );

        validator.AddCondition(
            "5. pls unban ALL users that have been banned for over a month!",
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
        //Canvas.ForceUpdateCanvases();
        

        if (user != null)
        {
            // red ring stuff
            bool correct = validator.Validate(user, day.Date);
            if (correct != decision)
            {
                if (correct)
                {
                    if (validator.DateCheck(user, day.Date))
                    {
                        BrokenRuleText.text = "User had been banned for a month already...";
                    }
                    else
                    {
                    BrokenRuleText.text = "No Rules Broken";
                    }
                } 
                else 
                {
                    BrokenRuleText.text = validator.GetBrokenRules(user, day.Date);
                }
                RedRing.SetActive(true);
            }
            else
            {
                RedRing.SetActive(false);
            }
            if (valRoutine != null)
            {
                StopCoroutine(valRoutine);
            }
            Canvas.ForceUpdateCanvases();
            BrokenRuleText.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
            BrokenRuleText.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;

            AfterAppeal?.Emit(correct == decision);
        }
        
        valRoutine = StartCoroutine(RedRingOff());
        MoveToNextUser();
    }

    IEnumerator RedRingOff()
    {
        yield return new WaitForSeconds(MistakeTime);
        RedRing.SetActive(false);
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
