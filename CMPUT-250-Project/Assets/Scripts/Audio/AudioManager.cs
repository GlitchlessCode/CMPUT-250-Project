using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AudioBusDefinition
{
    public AudioGameEvent AudioClipBus;
    public FloatGameEvent BusVolume;

    public bool IsValid
    {
        get => BusVolume != null && AudioClipBus != null;
    }
}

[Serializable]
public struct PlayOnStartAudio
{
    public Audio audio;
    public AudioGameEvent bus;
}

public class AudioManager : Subscriber
{
    public AudioSource AudioSourcePrefab;

    [Header("Audio Buses")]
    public List<AudioBusDefinition> Buses = new List<AudioBusDefinition>();

    [Header("Startup Audio")]
    public List<PlayOnStartAudio> PlayOnStart = new List<PlayOnStartAudio>();

    private List<AudioBus> buses;
    private Queue<AudioSource> sources;

    public override void Subscribe()
    {
        if (AudioSourcePrefab == null)
        {
            return;
        }

        buses = new List<AudioBus>();
        sources = new Queue<AudioSource>();

        foreach (AudioBusDefinition def in Buses)
        {
            if (def.IsValid)
            {
                buses.Add(new AudioBus(def, GetAudioSource, ReturnAudioSource, this));
            }
        }
    }

    public override void AfterSubscribe()
    {
        foreach (PlayOnStartAudio audio in PlayOnStart)
        {
            if (audio.audio.clip != null)
            {
                audio.bus?.Emit(audio.audio);
            }
        }
    }

    public AudioSource GetAudioSource()
    {
        if (sources.Count > 0)
        {
            return sources.Dequeue();
        }
        else
        {
            return Instantiate<AudioSource>(AudioSourcePrefab, gameObject.transform);
        }
    }

    public void ReturnAudioSource(AudioSource source)
    {
        sources.Enqueue(source);
    }

    void OnDestroy()
    {
        foreach (AudioBus bus in buses)
        {
            bus.Unsubscribe();
        }
    }
}

struct ActiveAudio
{
    public AudioSource source;
    public Audio audio;

    public ActiveAudio(AudioSource sourceIn, Audio audioIn)
    {
        source = sourceIn;
        audio = audioIn;
    }
}

class AudioBus
{
    private AudioGameEvent clipBus;
    private FloatGameEvent volumeAdjuster;

    private List<ActiveAudio> actives;
    private float volume;

    private Func<AudioSource> fetcher;
    private Action<AudioSource> returner;

    private MonoBehaviour executor;

    public AudioBus(
        AudioBusDefinition definition,
        Func<AudioSource> sourceFetcher,
        Action<AudioSource> sourceReturner,
        MonoBehaviour coroutineExecutor
    )
    {
        clipBus = definition.AudioClipBus;
        volumeAdjuster = definition.BusVolume;
        actives = new List<ActiveAudio>();
        volume = 1.0f;
        fetcher = sourceFetcher;
        returner = sourceReturner;
        executor = coroutineExecutor;

        clipBus.Subscribe(OnRecieveClip);
        volumeAdjuster.Subscribe(OnSetVolume);
    }

    public void Unsubscribe()
    {
        foreach (ActiveAudio active in actives)
        {
            active.source.Stop();
        }
        executor = null;
        fetcher = null;
        returner = null;
        clipBus.Unsubscribe(OnRecieveClip);
        volumeAdjuster.Unsubscribe(OnSetVolume);
    }

    private void OnSetVolume(float newVolume)
    {
        if (newVolume < 0.0f || newVolume > 1.0f)
        {
            return;
        }

        volume = newVolume;
        foreach (ActiveAudio active in actives)
        {
            active.source.volume = Mathf.Clamp(active.audio.volume, 0.0f, 1.0f) * newVolume;
        }
    }

    private void OnRecieveClip(Audio audio)
    {
        AudioSource source = fetcher?.Invoke();

        ActiveAudio active = new ActiveAudio(source, audio);

        actives.Add(active);
        source.clip = audio.clip;
        source.volume = Mathf.Clamp(audio.volume, 0.0f, 1.0f) * volume;
        source.loop = audio.loop;
        if (!audio.pitchOverride.active)
        {
            float randPitch = UnityEngine.Random.Range(-0.5f, 2.0f);
            source.pitch = 1.0f + randPitch * audio.pitchVariance;
        }
        else
        {
            source.pitch = audio.pitchOverride.value;
        }
        source.Play();

        if (!audio.loop)
        {
            executor.StartCoroutine(WaitForReset(active));
        }
    }

    private IEnumerator WaitForReset(ActiveAudio active)
    {
        yield return new WaitForSeconds(active.audio.clip.length);
        actives.Remove(active);
        returner?.Invoke(active.source);
    }
}
