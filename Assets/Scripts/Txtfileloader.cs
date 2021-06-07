using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


public class Txtfileloader : MonoBehaviour
{

   
    /*
    string path = @"C:\Users\kumav\Desktop\HCMC Cemetery\Assets\Resources\TotalCount.txt";
    ArrayList entries = new ArrayList();


    // Start is called before the first frame update
    void Start()
    {
        loadFile(path);
        for (int i = 0; i < entries.Count; i++)
        {
            string[] ss = (string[])entries[i];
            for (int j = 0; j < ss.Length; j++)
                Debug.Log(ss[j]);
        } 


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadFile(string filen)
    {
        entries.Clear();
        string line;
        StreamReader input_str = new StreamReader(filen);

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

        
    */
}
