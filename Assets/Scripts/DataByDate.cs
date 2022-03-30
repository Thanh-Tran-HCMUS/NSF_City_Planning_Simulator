using Michsky.UI.Shift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void datechangehandler();
public class DataByDate : MonoBehaviour
{
    private static string date = "Jul08";
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
        if (date== "Jul08")
        {
            gameObject.GetComponent<SwitchManager>().isOn = false;
            gameObject.GetComponent<Animator>().Play("Switch Off");
            //gameObject.GetComponent<SwitchManager>().AnimateSwitch();
        }
        else if (date =="May23")
        {
            gameObject.GetComponent<SwitchManager>().isOn = true;
            gameObject.GetComponent<Animator>().Play("Switch On");
            //  gameObject.GetComponent<SwitchManager>().AnimateSwitch();
        }
    }
    public void clickOn()
    {
        Date = "May23";
    }
    public void clickOff()
    {
        Date = "Jul08";
    }


    // Update is called once per frame
    void Update()
    {
      
    }
}
