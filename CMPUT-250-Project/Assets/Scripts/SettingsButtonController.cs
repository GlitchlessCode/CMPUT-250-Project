using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonController : Subscriber
{
    [Header("Buttons")]
    public Button SettingsOpenButton;
    public Button SettingsCloseButton;

    [Header("Panel")]
    public GameObject SettingsPanel;

    [Header("Audio")]
    public Audio TabSwitch;

    [Header("Event Listeners")]
    public AudioGameEvent AudioBus;
    private bool canUpdate = true;

    void Start()
    {
        SettingsOpenButton.onClick.AddListener(OnSettingsOpen);
        SettingsCloseButton.onClick.AddListener(OnSettingsClosed);
    }

    void OnSettingsOpen()
    {
        SettingsPanel.SetActive(true);
        AudioBus?.Emit(TabSwitch);
    }

    void OnSettingsClosed()
    {
        SettingsPanel.SetActive(false);
        AudioBus?.Emit(TabSwitch);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            if (canUpdate && !SettingsPanel.activeSelf)
            {
                OnSettingsOpen();
                StartCoroutine(DelayAction(0.5f));
            }
            else if (canUpdate && SettingsPanel.activeSelf)
            {
                OnSettingsClosed();
                StartCoroutine(DelayAction(0.5f));
            }
        }
    }

    IEnumerator DelayAction(float time)
    {
        canUpdate = false;
        yield return new WaitForSeconds(time);
        canUpdate = true;
    }
}
