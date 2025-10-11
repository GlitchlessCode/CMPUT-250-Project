using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CoupledEventSequence
{
    public UnitGameEvent trigger;
    public DirectMessageSequenceDefinition sequence;
}

public class DirectMessageManager : Subscriber
{
    [Header("Pools")]
    public DirectMessagePoolDefinition SuccessMessages;
    public DirectMessagePoolDefinition FailureMessages;

    [Header("Sequences")]
    public List<CoupledEventSequence> MessageSequences;

    [Header("Event Listeners")]
    public BoolGameEvent AfterAppeal;

    [Header("Events")]
    public DirectMessageGameEvent MessageTarget;

    private InternalDirectMessagePool successMessages;
    private InternalDirectMessagePool failureMessages;
    private List<InternalDirectMessageSequence> messageSequences;

    protected override void Subscribe()
    {
        successMessages = new InternalDirectMessagePool(SuccessMessages);
        failureMessages = new InternalDirectMessagePool(FailureMessages);
        messageSequences = new List<InternalDirectMessageSequence>();
        foreach (CoupledEventSequence coupled in MessageSequences)
        {
            if (coupled.sequence != null && coupled.trigger != null)
            {
                InternalDirectMessageSequence seq = new InternalDirectMessageSequence(
                    coupled.sequence
                );
                messageSequences.Add(seq);
                coupled.trigger.Subscribe(CreateOnSequenceTrigger(seq));
            }
        }

        AfterAppeal?.Subscribe(OnAfterAppeal);
    }

    void OnAfterAppeal(bool success)
    {
        if (success)
        {
            MessageTarget?.Emit(successMessages.GetRandomMessage());
        }
        else
        {
            MessageTarget?.Emit(failureMessages.GetRandomMessage());
        }
    }

    System.Action CreateOnSequenceTrigger(InternalDirectMessageSequence seq)
    {
        void OnSequenceTrigger()
        {
            foreach (DirectMessage msg in seq.GetMessages())
            {
                MessageTarget?.Emit(msg);
            }
        }
        return OnSequenceTrigger;
    }
}

class InternalDirectMessagePool
{
    List<DirectMessage> messages;

    public InternalDirectMessagePool(DirectMessagePoolDefinition def)
    {
        messages = def.GetMessages();
    }

    public DirectMessage GetRandomMessage()
    {
        System.Random rand = new System.Random();
        return messages[rand.Next(messages.Count)];
    }
}

class InternalDirectMessageSequence
{
    List<DirectMessage> messages;

    public InternalDirectMessageSequence(DirectMessageSequenceDefinition def)
    {
        messages = def.GetMessages();
    }

    public List<DirectMessage> GetMessages()
    {
        return messages;
    }
}
