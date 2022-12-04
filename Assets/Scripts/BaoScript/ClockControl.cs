using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ClockControl : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI ClockUI;
    [SerializeField]
    private GameObject day;
    [SerializeField]
    private GameObject night;

    string hour;
    string minute;
    string second;
    string timeOfday;

    bool reset;
    bool isDay;
    int date;

    private float timeScale;
    private DateTime currentTime;
    private float startHour = 12;

    void Start()
    {
        ResetClock();
        Debug.Log(date);
    }
    void Update() 
    {
        timeScale = Time.timeScale;
        CountTime(timeScale);
        CheckActive();
        UpdateTime();
       
    }
    void CountTime(float Tmulti)
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * Tmulti);
    }
    void UpdateTime()
    {
        if (ClockUI == null)
        {
            Debug.Log("ClockUI doesn't exist");
        }
        ClockUI.text = currentTime.ToString("hh:mm:ss") + " " + timeOfday; // 24 hour fomat = HH, 12 hour format hh
    }
    void CheckActive()
    {
        if (day != null && night != null)
        {
            if (day.activeSelf)
            {
                timeOfday = "PM";
                isDay = true;
            }
            if (night.activeSelf)
            {
                timeOfday = "AM";
                isDay = false;
            }
        }
        else
        {
            Debug.Log("check timer ui active object");
        }
    }

    void ClockLoop()//still need to fix clock loop !!!
    {
    }
    public void ResetClock()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        date = StringToInt(currentTime.ToString("dd")); //"dd/MM/yyyy"
    }

    private int StringToInt(string s)
    {
        return int.Parse(s);
    }
}
