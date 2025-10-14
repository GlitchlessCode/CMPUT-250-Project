using System;
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

    private bool loadedSuccessMessages = false;
    private InternalDirectMessagePool successMessages;
    private bool loadedFailureMessages = false;
    private InternalDirectMessagePool failureMessages;
    private List<InternalDirectMessageSequence> messageSequences;

    private Queue<bool> queuedAppeals;
    private Dictionary<Guid, int> queuedSequences;

    protected override void Subscribe()
    {
        AfterAppeal?.Subscribe(OnAfterAppealQueued);

        queuedAppeals = new Queue<bool>();
        StartCoroutine(
            SuccessMessages.GetMessages(
                (messages) =>
                {
                    successMessages = new InternalDirectMessagePool(messages);
                    loadedSuccessMessages = true;
                    catchUpAppeals();
                }
            )
        );
        StartCoroutine(
            FailureMessages.GetMessages(
                (messages) =>
                {
                    failureMessages = new InternalDirectMessagePool(messages);
                    loadedFailureMessages = true;
                    catchUpAppeals();
                }
            )
        );

        queuedSequences = new Dictionary<Guid, int>();
        messageSequences = new List<InternalDirectMessageSequence>();
        foreach (CoupledEventSequence coupled in MessageSequences)
        {
            if (coupled.sequence != null && coupled.trigger != null)
            {
                Guid id = Guid.NewGuid();
                Action action = CreateOnSequenceTriggerQueued(id);
                StartCoroutine(
                    coupled.sequence.GetMessages(
                        (messages) =>
                        {
                            InternalDirectMessageSequence seq = new InternalDirectMessageSequence(
                                messages
                            );
                            messageSequences.Add(seq);
                            catchUpSequence(id, seq, action, coupled.trigger);
                        }
                    )
                );
                queuedSequences[id] = 0;

                coupled.trigger.Subscribe(action);
            }
        }
    }

    void OnAfterAppealQueued(bool success)
    {
        queuedAppeals.Enqueue(success);
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

    void catchUpAppeals()
    {
        if (loadedSuccessMessages && loadedFailureMessages)
        {
            if (queuedAppeals.Count > 0)
            {
                foreach (bool success in queuedAppeals)
                {
                    OnAfterAppeal(success);
                }
                queuedAppeals.Clear();
            }

            AfterAppeal?.Subscribe(OnAfterAppeal);
            AfterAppeal?.Unsubscribe(OnAfterAppealQueued);
        }
    }

    Action CreateOnSequenceTriggerQueued(Guid id)
    {
        void OnSequenceTriggerQueued()
        {
            queuedSequences[id] += 1;
        }

        return OnSequenceTriggerQueued;
    }

    Action CreateOnSequence(InternalDirectMessageSequence seq)
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

    void catchUpSequence(
        Guid id,
        InternalDirectMessageSequence seq,
        Action old_action,
        UnitGameEvent trigger
    )
    {
        Action action = CreateOnSequence(seq);
        for (int count = 0; count < queuedSequences[id]; count++)
            action();
        queuedSequences[id] = -1;
        trigger.Subscribe(action);
        trigger.Unsubscribe(old_action);
    }
}

class InternalDirectMessagePool
{
    List<DirectMessage> messages;

    public InternalDirectMessagePool(List<DirectMessage> messagesIn)
    {
        messages = messagesIn;
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

    public InternalDirectMessageSequence(List<DirectMessage> messagesIn)
    {
        messages = messagesIn;
    }

    public List<DirectMessage> GetMessages()
    {
        return messages;
    }
}
