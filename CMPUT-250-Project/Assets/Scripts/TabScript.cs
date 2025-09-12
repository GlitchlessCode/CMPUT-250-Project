using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TabScript : MonoBehaviour
{
    public ToggleGroup toggleGroup;
    public Toggle banRequests;
    public Toggle Rules;
    public Toggle DMS;
    // Start is called before the first frame update
    void Start()
    {
        toggleGroup.RegisterToggle(banRequests);
        toggleGroup.RegisterToggle(Rules);
        toggleGroup.RegisterToggle(DMS);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ToggleValueChanged(Toggle change)
    {
        
    }
}
