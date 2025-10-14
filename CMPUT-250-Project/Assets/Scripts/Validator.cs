using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Validator
{
    // Dictionary of conditions with their string descriptions
    private Dictionary<string, Func<UserEntry?, bool>> _conditions =
        new Dictionary<string, Func<UserEntry?, bool>>();

    // Method to add conditions with a string description
    public void AddCondition(string description, Func<UserEntry?, bool> condition)
    {
        _conditions[description] = condition;
    }

    // Method to check if a UserEntry satisfies all conditions
    public bool Validate(UserEntry? user)
    {
        foreach (var condition in _conditions)
        {
            if (!condition.Value(user))
            {
                Debug.Log("Failed " + condition.Key);
                return false;
            }
        }
        return true;
    }

    // Method to remove a condition based on its description
    public bool RemoveCondition(string description)
    {
        return _conditions.Remove(description);
    }

    // message checks

    public bool messagesContain(UserEntry? user, string text)
    {
        return user.Value.messages.Any<string>(
            (msg) => Regex.IsMatch(msg, $".*{Regex.Escape(text)}.*")
        );
    }

    public bool messageRepeats(UserEntry? user, int reps)
    {
        string text = @".*(.)\1{" + (reps - 1) + ",}.*";

        foreach (string message in user.Value.messages)
        {
            if (Regex.IsMatch(message.ToLower(), text))
            {
                return false;
            }
        }

        return true;
    }

    public bool messageLengthCheck(UserEntry? user, string check, int length)
    {
        foreach (string message in user.Value.messages)
        {
            switch (check)
            {
                case "<=":
                    if (message.Length > length)
                    {
                        return false;
                    }
                    break;
                case "<":
                    if (message.Length >= length)
                    {
                        return false;
                    }
                    break;
                case ">=":
                    if (message.Length < length)
                    {
                        return false;
                    }
                    break;
                case ">":
                    if (message.Length <= length)
                    {
                        return false;
                    }
                    break;
                case "==":
                    if (message.Length != length)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    public bool numberMessages(UserEntry? user, string check, int num)
    {
        int n = 0;
        foreach (string message in user.Value.messages)
        {
            n++;
        }

        switch (check)
        {
            case "<=":
                if (n > num)
                {
                    return false;
                }
                break;
            case "<":
                if (n >= num)
                {
                    return false;
                }
                break;
            case ">=":
                if (n < num)
                {
                    return false;
                }
                break;
            case ">":
                if (n <= num)
                {
                    return false;
                }
                break;
            case "==":
                if (n != num)
                {
                    return false;
                }
                break;
            default:
                break;
        }
        return true;
    }

    // general string checks

    public bool stringContains(string s, string text)
    {
        return (Regex.IsMatch(s.ToLower(), $".*{Regex.Escape(text)}.*"));
    }

    public bool stringRepeats(string s, int reps)
    {
        return (Regex.IsMatch(s.ToLower(), @".*(.)\1{" + (reps - 1) + ",}.*"));
    }

    public bool stringLengthCheck(string s, string check, int length)
    {
        switch (check)
        {
            case "<=":
                if (s.Length > length)
                {
                    return false;
                }
                break;
            case "<":
                if (s.Length >= length)
                {
                    return false;
                }
                break;
            case ">=":
                if (s.Length < length)
                {
                    return false;
                }
                break;
            case ">":
                if (s.Length <= length)
                {
                    return false;
                }
                break;
            case "==":
                if (s.Length != length)
                {
                    return false;
                }
                break;
            default:
                break;
        }

        return true;
    }

    // combine rule text
    public string GetConditionText()
    {
        // Join all condition descriptions into one string, separated by commas or any other separator you prefer
        return string.Join("\n", _conditions.Keys);
    }
}
