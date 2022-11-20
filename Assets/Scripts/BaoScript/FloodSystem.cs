using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodSystem : MonoBehaviour
{
    private CameraData dataScript;
    private CameraLocator locatorScript;
    string filename = "cameralocation.json";

    public List<CameraData> camList = new List<CameraData>();
    // Start is called before the first frame update
    void Start()
    {
        //camList = locatorScript.FromJSON<CameraData>(filename);
    }
     
    // Update is called once per frame
    void Update()
    {
        
    }
}
