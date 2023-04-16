using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

[Serializable]
public class CameraData
{
    // script in canvas video object
    public string Name;
    public float[] Position;
    public bool Flood;

    public CameraData(GameObject cam)
    {
        Name = Regex.Match(cam.name, @"\d+").Value;
        Position = new float[3];
        Position[0] = cam.transform.position.x;
        Position[1] = cam.transform.position.y;
        Position[2] = cam.transform.position.z;
        Flood = false;
    }

   
}
