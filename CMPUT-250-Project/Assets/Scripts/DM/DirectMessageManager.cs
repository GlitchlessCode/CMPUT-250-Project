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
    public DirectMessagePoolDefinition GoodMessages;
    public DirectMessagePoolDefinition BadMessages;
    public DirectMessagePoolDefinition GettingGoodMessages;
    public DirectMessagePoolDefinition GettingBadMessages;

    [Header("Sequences")]
    public List<CoupledEventSequence> MessageSequences;

    [Header("Audio")]
    public Audio DMArrivedAudio;

    [Header("Event Listeners")]
    public BoolGameEvent AfterAppeal;

    [Header("Events")]
    public DirectMessageGameEvent MessageTarget;
    public AudioGameEvent AudioBus;

    private int lastState = 0;
    private int state = 0;
    private bool loadedGoodMessages = false;
    private InternalDirectMessagePool goodMessages;
    private bool loadedBadMessages = false;
    private InternalDirectMessagePool badMessages;
    private bool loadedGettingGoodMessages = false;
    private InternalDirectMessagePool gettingGoodMessages;
    private bool loadedGettingBadMessages = false;
    private InternalDirectMessagePool gettingBadMessages;
    private List<InternalDirectMessageSequence> messageSequences;

    private Queue<bool> queuedAppeals;
    private Dictionary<Guid, int> queuedSequences;

    private Queue<DirectMessage> queuedMessages = new Queue<DirectMessage>();
    private bool isRunningQueue = false;

    public override void Subscribe()
    {
        AfterAppeal?.Subscribe(OnAfterAppealQueued);

        queuedAppeals = new Queue<bool>();
        StartCoroutine(
            GoodMessages.GetMessages(
                (messages) =>
                {
                    goodMessages = new InternalDirectMessagePool(messages);
                    loadedGoodMessages = true;
                    catchUpAppeals();
                }
            )
        );
        StartCoroutine(
            BadMessages.GetMessages(
                (messages) =>
                {
                    badMessages = new InternalDirectMessagePool(messages);
                    loadedBadMessages = true;
                    catchUpAppeals();
                }
            )
        );
        StartCoroutine(
            GettingGoodMessages.GetMessages(
                (messages) =>
                {
                    gettingGoodMessages = new InternalDirectMessagePool(messages);
                    loadedGettingGoodMessages = true;
                    catchUpAppeals();
                }
            )
        );
        StartCoroutine(
            GettingBadMessages.GetMessages(
                (messages) =>
                {
                    gettingBadMessages = new InternalDirectMessagePool(messages);
                    loadedGettingBadMessages = true;
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

    void OnAfterAppeal(bool currentCorrect)
    {
        float frequency = UnityEngine.Random.Range(0, 1);
        if (currentCorrect)
        {
            state = Math.Min(state + 1, 2);
        }
        else
        {
            state = Math.Max(state - 1, -2);
        }
        if (state == 0 && lastState > 0) // getting bad
        {
            QueueMessage(gettingBadMessages.GetRandomMessage());
        }
        else if (state == 0 && lastState < 0) // getting good
        {
            QueueMessage(gettingGoodMessages.GetRandomMessage());
        }
        else if (state > 0 && lastState > 0) // good
        {
            if (frequency <= 0.3)
            {
                QueueMessage(goodMessages.GetRandomMessage());
            }
        }
        else if (state < 0 && lastState < 0) // bad
        {
            if (frequency <= 0.3)
            {
                QueueMessage(badMessages.GetRandomMessage());
            }
        }
    }

    void catchUpAppeals()
    {
        if (
            loadedGoodMessages
            && loadedBadMessages
            && loadedGettingGoodMessages
            && loadedGettingBadMessages
        )
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
                QueueMessage(msg);
            }
        }

        return OnSequenceTrigger;
    }

    void QueueMessage(DirectMessage message)
    {
        queuedMessages.Enqueue(message);
        if (!isRunningQueue)
        {
            StartCoroutine(RunQueue());
        }
    }

    IEnumerator RunQueue()
    {
        isRunningQueue = true;

        yield return new WaitForSeconds(0.7f);

        while (queuedMessages.Count > 0)
        {
            DirectMessage message = queuedMessages.Dequeue();
            MessageTarget?.Emit(message);
            if (DMArrivedAudio.clip != null)
            {
                AudioBus?.Emit(DMArrivedAudio);
            }
            yield return new WaitForSeconds(0.7f);
        }

        isRunningQueue = false;
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
