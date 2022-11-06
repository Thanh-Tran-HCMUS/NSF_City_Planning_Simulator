using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraData
{
    public string camName;
    public float[] camPosition;

    public CameraData(GameObject cam)
    {
        camName = cam.name;
        camPosition = new float[3];
        camPosition[0] = cam.transform.position.x;
        camPosition[1] = cam.transform.position.y;
        camPosition[2] = cam.transform.position.z;
    }
}
