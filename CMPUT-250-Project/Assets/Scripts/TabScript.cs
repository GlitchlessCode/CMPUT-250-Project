using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;


public class TabScript : MonoBehaviour
{
    Toggle banAppeal;
    Toggle rules;
    Toggle dms;
    public static ToggleGroup toggleGroup;
    protected int currentTabIndex { get; set; }
    Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
    ToggleUI uiSwitch = new ToggleUI();

    // Start is called before the first frame update
    void Start()
    {
        
        int i = 0;
        foreach (Toggle toggle in toggles) {
            toggle.onValueChanged.AddListener(delegate { SetTabState(toggle, toggle.isOn); });
        }

    }
    protected void SetTabState(Toggle toggle, bool isSelected)
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

