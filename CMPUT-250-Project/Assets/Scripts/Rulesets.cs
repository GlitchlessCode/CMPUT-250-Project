using System.Text.RegularExpressions;

public interface IRuleset
{
    void AddRules(Validator validator);
}

public class EmptyRuleset : IRuleset
{
    public void AddRules(Validator validator)
    {
        UnityEngine.Debug.LogWarning("Added empty ruleset, is that a mistake?");
        // No-Op
    }
}

public class Day1Rules : IRuleset
{
    public void AddRules(Validator validator)
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
                return validator.messagesContain(currentUser, @"[A-Z]");
            }
        );

        validator.AddCondition(
            "5. pls unban ALL users that have been banned for over a month!",
            (currentUser) =>
            {
                return true;
            }
        );
    }
}

public class Day2Rules : IRuleset
{
    public void AddRules(Validator validator)
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
            "3. MINIMUM 3 words in each chat message!",
            (currentUser) =>
            {
                return validator.wordsPerMessage(currentUser, ">", 3);
            }
        );

        validator.AddCondition(
            "4. no full chat logs in ENTIRELY lowercase!",
            (currentUser) =>
            {
                return validator.messagesContain(currentUser, @"^[^a-z]*$");
            }
        );

        validator.AddCondition(
            "5. do NOT share personal information!",
            (currentUser) =>
            {
                foreach (string msg in currentUser.Value.messages)
                {
                    if (Regex.IsMatch(msg.ToLower(), @"[0-9]{3}"))
                    {
                        return false;
                    }
                }
                return true;
            }
        );

        validator.AddCondition(
            "6. NO links in user bios or chat!",
            (currentUser) =>
            {
                return validator.messagesContain(currentUser, @"https?://");
            }
        );

        validator.AddCondition(
            "7. pls unban ALL users that have been banned for over a month!",
            (currentUser) =>
            {
                return true;
            }
        );
    }
}
