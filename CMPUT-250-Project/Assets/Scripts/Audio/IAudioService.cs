// IAudioService.cs
public interface IAudioService
{
    void PlaySFX(string key, float volume = 1f, float pitch = 1f);
    void PlaySFXAt(string key, UnityEngine.Vector3 pos, float volume = 1f, float pitch = 1f);
    void PlayMusic(string key, float fadeSeconds = 0.5f, bool loop = true);
    void StopMusic(float fadeSeconds = 0.5f);
    void SetMasterVolume(float linear01);
    void SetMusicVolume(float linear01);
    void SetSFXVolume(float linear01);
}
