using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : Subscriber
{
    [Header("Tabs")]
    public Toggle appealTab;
    public Toggle rulesTab;
    public Toggle dmsTab;
    public Toggle settingsTab;

    [Header("Panels")]
    public GameObject appealPanel;
    public GameObject rulesPanel;
    public GameObject dmsPanel;
    public GameObject settingsPanel;

    [Header("Events")]
    public BoolGameEvent DMTabClick;
    public BoolGameEvent AppealPanelActive;

    [Header("Event Listeners")]
    public UnitGameEvent AsyncComplete;

    private Dictionary<Toggle, GameObject> tabsDictionary;
    private bool asyncComplete;

    public override void Subscribe()
    {
        AsyncComplete?.Subscribe(OnAsyncComplete);
    }

    public override void AfterSubscribe()
    {
        appealTab.onValueChanged.AddListener(ActiveTab);
        rulesTab.onValueChanged.AddListener(ActiveTab);
        dmsTab.onValueChanged.AddListener(ActiveTab);
        settingsTab.onValueChanged.AddListener(ActiveTab);

        tabsDictionary = new Dictionary<Toggle, GameObject>();
        tabsDictionary.Add(appealTab, appealPanel);
        tabsDictionary.Add(rulesTab, rulesPanel);
        tabsDictionary.Add(dmsTab, dmsPanel);
        tabsDictionary.Add(settingsTab, settingsPanel);
    }

    private void ActiveTab(bool arg0)
    {
        TabSwap(tabsDictionary);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
        {
            rulesTab.isOn = true;
            DMTabClick?.Emit(false);
        }
        else if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
        {
            appealTab.isOn = true;
            DMTabClick?.Emit(false);
        }
        else if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
        {
            dmsTab.isOn = true;
            DMTabClick?.Emit(true);
        }
        else if (Input.GetKey(KeyCode.O))
        {
            settingsTab.isOn = true;
            DMTabClick?.Emit(false);
        }
    }

    void OnAsyncComplete()
    {
        asyncComplete = true;
        if (appealPanel.activeSelf)
        {
            AppealPanelActive?.Emit(true);
        }
    }

    void TabSwap(Dictionary<Toggle, GameObject> tabsDictionary)
    {
        bool OnDMTab = false;

        foreach (KeyValuePair<Toggle, GameObject> tab in tabsDictionary)
        {
            if (tab.Key.isOn)
            {
                tab.Value.gameObject.SetActive(true);
            }
            else
            {
                tab.Value.gameObject.SetActive(false);
            }

            if (tab.Key == dmsTab && tab.Key.isOn)
            {
                OnDMTab = true;
            }
        }

        DMTabClick?.Emit(OnDMTab);
        if (asyncComplete)
        {
            AppealPanelActive?.Emit(appealPanel.activeSelf);
        }
    }
}
