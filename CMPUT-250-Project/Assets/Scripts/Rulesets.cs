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
    }
}
