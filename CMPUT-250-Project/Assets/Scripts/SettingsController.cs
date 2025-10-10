using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI musicSliderText = null;
    [SerializeField] private TextMeshProUGUI soundSliderText = null;

    [SerializeField] private float maxSliderAmount = 100.0f;

    public void MusicSliderChange(float value)
    {
        float localValue = value * maxSliderAmount;
        musicSliderText.text = localValue.ToString("0");
    }

    public void SoundSliderChange(float value)
    {
        float localValue = value * maxSliderAmount;
        soundSliderText.text = localValue.ToString("0");
    }
}
