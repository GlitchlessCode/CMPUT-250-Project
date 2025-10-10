using System.Collections;
using System.Collections.Generic;
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

    [Header("Events")]
    public FloatGameEvent ChangeMusic;
    public FloatGameEvent ChangeSound;

    public void MusicValueChange(float value)
    {
        changeValue(value, ChangeMusic, musicSliderText);
    }

    public void SoundValueChange(float value)
    {
        changeValue(value, ChangeSound, soundSliderText);
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
