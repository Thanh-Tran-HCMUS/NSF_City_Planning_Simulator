using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

public class SelectCamera : MonoBehaviour
{
    public static long count1 = 50;
    public static long count2 = 0;
    public Material lightTraffic;
    public Material heavyTraffic;
    public Material normalTraffic;
    public Material noTraffic;
    Transform selection; // select the transform of line
    public Transform leftLine;
    public Transform rightLine;
    private int fastFlow = 5;
    private int normalFlow = 11;
    public int delaytime; //in seconds
    int count = 0;
    float time = 0f;
    Highlight_Line hl;
    public string path;
    public ArrayList entries = new ArrayList();
 
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Cam"))
                {
                    Debug.Log(hit.transform.gameObject);
                  
                    OpenExplorer();
                    loadFile(path);
                    hl.Update();

                }

            }
           
        }


    }

    public void OpenExplorer()
    {
        //Application.OpenURL(@"C:\Users\kumav\Desktop\HCMC Cemetery\Assets\Resources\Cam-45");
        //path = EditorUtility.OpenFilePanel("Camera File .txt", "", "txt");
        //loadFile(path);
        
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


    public void forRightSide(Transform t)
    {
    }

    public void forLeftSide(Transform t)
    {
        selection = t;
    }


}
