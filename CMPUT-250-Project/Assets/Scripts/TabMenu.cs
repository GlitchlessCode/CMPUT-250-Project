using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class TabMenu : MonoBehaviour
{
    public Toggle appealTab;
    public Toggle rulesTab;
    public Toggle dmsTab;
    public GameObject appealPanel;
    public GameObject rulesPanel;
    public GameObject dmsPanel;

    void Start()
    {
        rulesTab.isOn = true;
        appealTab.onValueChanged.AddListener(appealActive);
        rulesTab.onValueChanged.AddListener(rulesActive);
        dmsTab.onValueChanged.AddListener(dmsActive);
    }

    private void appealActive(bool arg0)
    {
        if (appealTab.isOn)
        {
            appealPanel.gameObject.SetActive(true);
            rulesPanel.gameObject.SetActive(false);
            dmsPanel.gameObject.SetActive(false);
        }
    }
    private void rulesActive(bool arg0)
    {
        if (rulesTab.isOn)
        {
            appealPanel.gameObject.SetActive(false);
            rulesPanel.gameObject.SetActive(true);
            dmsPanel.gameObject.SetActive(false);
        }
    }
    private void dmsActive(bool arg0)
    {
        if (dmsTab.isOn)
        {
            appealPanel.gameObject.SetActive(false);
            rulesPanel.gameObject.SetActive(false);
            dmsPanel.gameObject.SetActive(true);
        }
    }
    void Update(){

        if(Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
        {
            rulesTab.isOn = !rulesTab.isOn;
        }
        else if(Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
        {
            appealTab.isOn = !appealTab.isOn;
        }
        else if(Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
        {
            dmsTab.isOn = !dmsTab.isOn;
        }
    }

}