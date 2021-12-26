using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class UIDropDown : MonoBehaviour
{
    public Image MapImage;
    public Sprite NewImage;


    public TMP_Dropdown sceneDD; //Val 0 = D1 , Val 1 = TDC
    public TMP_Dropdown optionDD; 
    public TMP_Dropdown speedDD; // val 0 = 1x , val ++ 

    public Sprite[] allMap;

    void Start()
    {
        Default();
        NewImage = allMap[0];
        MapImage.sprite = NewImage;

        UICaller uiCaller = GetComponent<UICaller>();
        uiCaller.OnMKeyPressed += ChangeToRoadEditor;
    }
    
    private void ChangeToRoadEditor(object sender, EventArgs e)
    {
        Debug.Log("M pressed");
    }

    void Update()
    {
        if(sceneDD.value==0)
        {
            switch(optionDD.value)
            {
                case 1:
                    NewImage = allMap[1];
                    ChangeMap();
                    break;
                case 2:
                    NewImage = allMap[2];
                    ChangeMap();
                    break;
                default:
                    NewImage = allMap[0];
                    ChangeMap();
                    break;
            }
        }
        else if (sceneDD.value == 1)
        {
            switch (optionDD.value)
            {
                case 1:
                    NewImage = allMap[4];
                    ChangeMap();
                    break;
                case 2:
                    NewImage = allMap[5];
                    ChangeMap();
                    break;
                default:
                    NewImage = allMap[3];
                    ChangeMap();
                    break;
            }
        }
    }

    public void ChangeMap()
    {
        MapImage.sprite = NewImage;
    }

    public void Default()
    {
        sceneDD.value = 0;
        optionDD.value = 0;
        speedDD.value = 0;
    }
    
}
