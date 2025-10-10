using UnityEngine;

[System.Serializable]
public struct Audio
{
    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    public float volume;

    [Range(0.0f, 3.0f)]
    public float pitchVariance;

    public bool loop;

    public Audio(AudioClip clipIn, float volumeIn, float pitchVarianceIn, bool loopIn)
    {
        clip = clipIn;
        volume = volumeIn;
        pitchVariance = pitchVarianceIn;
        loop = loopIn;
    }
}

[CreateAssetMenu(menuName = "Game Events/AudioGameEvent")]
public class AudioGameEvent : GameEvent<Audio> { }
