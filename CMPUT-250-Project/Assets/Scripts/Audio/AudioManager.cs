using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour, IAudioService
{
    [Header("Mixer + Library")]
    public AudioMixer mixer;
    public AudioLibrary library;

    [Header("Music Sources")]
    public AudioSource musicA;
    public AudioSource musicB;

    [Header("SFX Pool")]
    public AudioSource[] sfxPool;
    int sfxIndex;

    public static IAudioService Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != (IAudioService)this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(string key, float volume = 1f, float pitch = 1f)
    {
        if (!library.TryGet(key, out var def) || def.clip == null) return;
        var src = NextSFX();
        ConfigureSource(src, def, volume, pitch, twoD: true);
        src.PlayOneShot(def.clip, def.defaultVolume * volume);
    }

    public void PlaySFXAt(string key, Vector3 pos, float volume = 1f, float pitch = 1f)
    {
        if (!library.TryGet(key, out var def) || def.clip == null) return;
        var src = NextSFX();
        ConfigureSource(src, def, volume, pitch, twoD: false);
        src.transform.position = pos;
        src.spatialBlend = Mathf.Max(def.spatialBlend, 1f);
        src.clip = def.clip;
        src.volume = def.defaultVolume * volume;
        src.pitch = def.defaultPitch * pitch;
        src.loop = false;
        src.Play();
    }

    public void PlayMusic(string key, float fadeSeconds = 0.5f, bool loop = true)
    {
        if (!library.TryGet(key, out var def) || def.clip == null) return;
        StopAllCoroutines();
        StartCoroutine(CrossfadeMusic(def.clip, fadeSeconds, loop));
    }

    public void StopMusic(float fadeSeconds = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutBoth(fadeSeconds));
    }

    public void SetMasterVolume(float linear01) => SetDb("MasterVol", linear01);
    public void SetMusicVolume(float linear01)  => SetDb("MusicVol",  linear01);
    public void SetSFXVolume(float linear01)    => SetDb("SFXVol",    linear01);

    // --- internals ---

    AudioSource NextSFX()
    {
        var src = sfxPool[sfxIndex];
        sfxIndex = (sfxIndex + 1) % sfxPool.Length;
        return src;
    }

    void ConfigureSource(AudioSource src, SoundDef def, float vol, float pit, bool twoD)
    {
        src.clip = null;
        src.loop = false;
        src.spatialBlend = twoD ? 0f : def.spatialBlend;
        src.pitch = Mathf.Clamp(def.defaultPitch * pit, 0.1f, 3f);
        src.volume = Mathf.Clamp01(def.defaultVolume * vol);
    }

    IEnumerator CrossfadeMusic(AudioClip newClip, float dur, bool loop)
    {
        var from = musicA.isPlaying ? musicA : musicB;
        var to   = musicA.isPlaying ? musicB : musicA;

        to.clip = newClip;
        to.loop = loop;
        to.volume = 0f;
        to.Play();

        float t = 0f;
        float fromStart = from.volume;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = (dur <= 0f) ? 1f : t / dur;
            to.volume   = Mathf.Lerp(0f, 1f, a);
            from.volume = Mathf.Lerp(fromStart, 0f, a);
            yield return null;
        }
        from.Stop();
        to.volume = 1f;
    }

    IEnumerator FadeOutBoth(float dur)
    {
        float t = 0f;
        float a0 = musicA.volume, b0 = musicB.volume;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = (dur <= 0f) ? 1f : t / dur;
            musicA.volume = Mathf.Lerp(a0, 0f, a);
            musicB.volume = Mathf.Lerp(b0, 0f, a);
            yield return null;
        }
        musicA.Stop(); musicB.Stop();
    }

    void SetDb(string param, float linear01)
    {
        if (mixer == null) return;
        mixer.SetFloat(param, LinearToDb(Mathf.Clamp01(linear01)));
    }

    float LinearToDb(float x) => (x <= 0.0001f) ? -80f : 20f * Mathf.Log10(x);
}

