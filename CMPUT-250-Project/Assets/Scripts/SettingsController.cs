using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI musicSliderText = null;
    [SerializeField] private TextMeshProUGUI soundSliderText = null;

    [SerializeField] private float maxSliderAmount = 100.0f;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider soundSlider;

    [Header("Events")]
    public FloatGameEvent ChangeMusic;
    public FloatGameEvent ChangeSound;

    void Start()
    {
        musicSlider.onValueChanged.AddListener(MusicValueChange);
        soundSlider.onValueChanged.AddListener(SoundValueChange);
    }

    public void MusicValueChange(float value)
    {
        ChangeMusic?.Emit(value);
    }

    public void SoundValueChange(float value)
    {
        ChangeSound?.Emit(value);
    }

    public void MusicSliderTextChange(float value)
    {
        float localValue = value * maxSliderAmount;
        musicSliderText.text = localValue.ToString("0");
    }

    public void SoundSliderTextChange(float value)
    {
        float localValue = value * maxSliderAmount;
        soundSliderText.text = localValue.ToString("0");
    }
}
