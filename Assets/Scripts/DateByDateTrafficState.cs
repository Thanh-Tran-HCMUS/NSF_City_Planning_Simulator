using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateByDateTrafficState : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (DataByDate.Date == "XeDong")
        {
            gameObject.GetComponent<HorizontalSelector>().index = 0;
            gameObject.GetComponent<HorizontalSelector>().UpdateUI();
        }
        else
        {
            if (DataByDate.Date == "XeVang")
            {
                gameObject.GetComponent<HorizontalSelector>().index = 1;
                gameObject.GetComponent<HorizontalSelector>().UpdateUI();
            }
        }
    }

    public void DateXeDong()
    {
        DataByDate.Date = "XeDong";
    }

    public void DateXeVang()
    {
        DataByDate.Date = "XeVang";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
