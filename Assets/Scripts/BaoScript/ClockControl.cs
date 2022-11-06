using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ClockControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ClockUI;

    void Update() 
    {
        ClockUI.text = System.DateTime.Now.ToString();
    }
}
