using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField]
    private Text musicSliderText = null;

    [SerializeField]
    private Text soundSliderText = null;

    [SerializeField]
    private float maxSliderAmount = 100.0f;

    [Header("Audio")]
    public Audio SoundSample;

    [Header("Events")]
    public FloatGameEvent ChangeMusic;
    public FloatGameEvent ChangeSound;
    public AudioGameEvent SoundSampleBus;

    private bool shouldPlaySample = true;

    public void MusicValueChange(float value)
    {
        changeValue(value, ChangeMusic, musicSliderText);
    }

    public void SoundValueChange(float value)
    {
        changeValue(value, ChangeSound, soundSliderText);
        if (SoundSample.clip != null && shouldPlaySample)
        {
            shouldPlaySample = false;
            SoundSampleBus?.Emit(SoundSample);
            StartCoroutine(ResetSamplePlayer());
        }
    }

    IEnumerator ResetSamplePlayer()
    {
        yield return new WaitForSeconds(0.08f);
        shouldPlaySample = true;
    }

    private void changeValue(float value, FloatGameEvent channel, Text sliderText)
    {
        channel?.Emit(value / maxSliderAmount);
        if (sliderText != null)
        {
            float localValue = value;
            sliderText.text = localValue.ToString("0");
        }
    }
}
