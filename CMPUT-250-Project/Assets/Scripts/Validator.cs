using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class Validator 
{
    // Dictionary of conditions with their string descriptions
    private Dictionary<string, Func<UserEntry?, bool>> _conditions = new Dictionary<string, Func<UserEntry?, bool>>();

    // Method to add conditions with a string description
    public void AddCondition(string description, Func<UserEntry?, bool> condition)
    {
        _conditions[description] = condition;
    }

    // Method to check if a UserEntry satisfies all conditions
    public bool Validate(UserEntry? user)
    {
        foreach (var condition in _conditions.Values)
        {
            if (!condition(user))
            {
                return false;
            }
        }
        return true;
    }

    public bool messagesContain(UserEntry? user, string text)
    {
        bool pass = true;
        text = @".*" + text + ".*";

        foreach (string message in user.Value.messages)
        {
            if (Regex.IsMatch(message.ToLower(), text))
            {
                pass = false; 
            }
        }

        return pass;
    }

}