using UnityEngine;

[System.Serializable]
public struct PitchOverride
{
    public bool active;

    [Range(0.0f, 5.0f)]
    public float value;

    public PitchOverride(bool activeIn, float valueIn)
    {
        active = activeIn;
        value = valueIn;
    }
}

[System.Serializable]
public struct Audio
{
    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    public float volume;

    [Range(0.0f, 3.0f)]
    public float pitchVariance;

    public bool loop;

    public PitchOverride pitchOverride;

    public Audio(AudioClip clipIn, float volumeIn, float pitchVarianceIn, bool loopIn)
    {
        clip = clipIn;
        volume = volumeIn;
        pitchVariance = pitchVarianceIn;
        loop = loopIn;
        pitchOverride = new PitchOverride(false, 1.0f);
    }
}

[CreateAssetMenu(menuName = "Game Events/AudioGameEvent")]
public class AudioGameEvent : GameEvent<Audio> { }
