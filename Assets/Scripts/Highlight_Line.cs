using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Text.RegularExpressions;

public class Highlight_Line : MonoBehaviour
{
    
    public static long count1 = 50;
    public static long count2 = 0;
    public Material lightTraffic;
    public Material heavyTraffic;
    public Material normalTraffic;
    public Material noTraffic;
    public Transform selection;     // select the transform of line
    private int fastFlow = 5;
    private int normalFlow = 11;
    SelectCamera scam;
    string path;
    RaycastHit hit;
    //string path = @"C:\Users\kumav\Desktop\HCMC Cemetery\Assets\Resources\TotalCount.txt";

    public int delaytime; //in seconds
    ArrayList entries = new ArrayList();
    int count = 0;
    float time = 0f;
    void Start()
    {
        


        // Debug.Log("hihi haha");
        //OpenExplorer();
        //loadFile(path);
        //  Debug.Log(entries.Count);

        /*  for (int i = 0; i < entries.Count; i++)
          {
              string[] ss = (string [])entries[i];
              for(int j = 0;j<ss.Length;j++)
                  Debug.Log(ss[j]);
          } */


    }



      public void OpenExplorer()
      {

          //Application.OpenURL(@"C:\Users\kumav\Desktop\HCMC Cemetery\Assets\Resources\Cam-45");
          path = EditorUtility.OpenFilePanel("Camera File .txt", "", "txt");
          loadFile(path);
      } 

    public void setHeavyTraffic()
    {
        var changecolor = selection.gameObject.transform;
        var chcolorRenderer = changecolor.GetComponent<Renderer>();
        chcolorRenderer.material = heavyTraffic;     //change to heavyTraffic
    }

    public void setNoTraffic()
    {
        var changecolor = selection.gameObject.transform;
        var chcolorRenderer = changecolor.GetComponent<Renderer>();
        chcolorRenderer.material = noTraffic;     //change to noTraffic
    }


    public void setNormalTraffic()
    {
        var changecolor = selection.gameObject.transform;
        var chcolorRenderer = changecolor.GetComponent<Renderer>();
        chcolorRenderer.material = normalTraffic;     //change to normalTraffic
    }


    public void setLightTraffic()
    {
        var changecolor = selection.gameObject.transform;
        var chcolorRenderer = changecolor.GetComponent<Renderer>();
        chcolorRenderer.material = lightTraffic;     //change to lightTraffic
    }


    public int roadside(int t)//t:time
    {

        int num = 0;

        if (selection.gameObject.name == "Traffic_Status_Line_R")
        {
            //Debug.Log("RRRRRRRRRRRRRRRRRRR");
            //for (int i = 0; i < entries.Count; i++)
            //{
            string[] ss = (string[])entries[t];
            num = Int32.Parse(ss[2]);
            // return num;
            // }
        }
        else if (selection.gameObject.name == "Traffic_Status_Line_L")
        {
           // Debug.Log("LLLLLLLLLLLLLLLLLLLLL");
            //  for (int i = 0; i < entries.Count; i++)
            // {
            string[] ss = (string[])entries[t];
            num = Int32.Parse(ss[1]);
            // return num;
            // }
        }
        return num;
    }

    public void loadFile(String filename)
    {
        entries.Clear();
        string line;
        StreamReader input_str = new StreamReader(filename);

        using (input_str)
        {
            do
            {
                line = input_str.ReadLine();
                if (line != null)
                {
                    string[] ss = line.Split(',');
                    entries.Add(ss);
                }
            }
            while (line != null);
            input_str.Close();
        }

    } 



    // Update is called once per frame
    public void Update()
    {
            if (Input.GetMouseButtonDown(0))
            {

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           
                if (Physics.Raycast(ray, out hit, 90))
                    if(gameObject == hit.transform.gameObject)
                    //if the hit point hits the current GameObject
                {

                    Debug.Log(hit.transform.name);

                    OpenExplorer();
                    loadFile(path);


                }
            } 
        

        // UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        time += Time.deltaTime;
        if (time >= delaytime)   //2
        {
            time = 0f;
            if (count > entries.Count)
            {
                count = 0;
            }
            else
            {
                count++;
            }


            int flow = roadside(count);

            if (flow <= fastFlow)
                setLightTraffic();
            else if (flow <= normalFlow)
                setNormalTraffic();
            else
            {
                setHeavyTraffic();
            }


        }


        return;


    }
}
