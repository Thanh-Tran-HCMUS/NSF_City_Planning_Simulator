//using Michsky.UI.Shift;
using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public delegate void datechangehandler();
public class DataByDate : MonoBehaviour
{
    private static string date = "May23";
    public static datechangehandler changehandler = null;

    public static string Date { get => date; 
        set 
        { 
            date = value; 
            changehandler?.Invoke(); 
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        //if (date== "May23")
        //{
        //    gameObject.GetComponent<SwitchManager>().isOn = false;
        //    gameObject.GetComponent<Animator>().Play("Switch Off");
        //    //gameObject.GetComponent<SwitchManager>().AnimateSwitch();
        //}


        if (date == "May23")
        {
            Debug.Log("1: " + gameObject.GetComponent<HorizontalSelector>().defaultIndex);
            gameObject.GetComponent<HorizontalSelector>().index = 0;
            gameObject.GetComponent<HorizontalSelector>().UpdateUI();
            Debug.Log("2: " + gameObject.GetComponent<HorizontalSelector>().defaultIndex);
        }
        else if (date == "Jul08")
        {
            gameObject.GetComponent<HorizontalSelector>().index = 1;
            gameObject.GetComponent<HorizontalSelector>().UpdateUI();

        }
        else if (date == "Jan01")
        {
            gameObject.GetComponent<HorizontalSelector>().index = 2;
            gameObject.GetComponent<HorizontalSelector>().UpdateUI();
        }
        else if (date == "Sep06")
        {
            gameObject.GetComponent<HorizontalSelector>().index = 3;
            gameObject.GetComponent<HorizontalSelector>().UpdateUI();
        }
    }
    public void clickOn()
    {
        Date = "Jul08";
    }
    public void clickOff()
    {
        Date = "May23";
    }
    public void dataJul08 ()
    {
        Date = "Jul08";
    }
    public void dataMay23()
    {
        Date = "May23";
    }
    public void dataJan01()
    {
        Date = "Jan01";
    }
    public void dataSep06()
    {
        Date = "Sep06";
    }
}
