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
        appealTab.onValueChanged.AddListener(appealTime);
        rulesTab.onValueChanged.AddListener(rulesTime);
        dmsTab.onValueChanged.AddListener(dmsTime);
    }

    private void appealTime(bool arg0)
    {
        if (appealTab.isOn)
        {
            appealPanel.gameObject.SetActive(true);
            rulesPanel.gameObject.SetActive(false);
            dmsPanel.gameObject.SetActive(false);
        }
    }
    private void rulesTime(bool arg0)
    {
        if (rulesTab.isOn)
        {
            appealPanel.gameObject.SetActive(false);
            rulesPanel.gameObject.SetActive(true);
            dmsPanel.gameObject.SetActive(false);
        }
    }
    private void dmsTime(bool arg0)
    {
        if (dmsTab.isOn)
        {
            appealPanel.gameObject.SetActive(false);
            rulesPanel.gameObject.SetActive(false);
            dmsPanel.gameObject.SetActive(true);
        }
    }

}