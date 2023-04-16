using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodSystem : MonoBehaviour
{
    public GameObject daywaterPrefab;
    [SerializeField] GameObject nightwaterPrefab;

    public string floodcam;// test flood location

    public CameraLocator script; //call script
    public Dictionary<string, CameraData> dictionary = new Dictionary<string, CameraData>();//create dictotionary type
    // radius componets adjustable 
    
    void Start()
    {
        foreach(var a in script.importCamList)
        {
            //dictionary[a.Name] = a;
            dictionary.Add(a.Name, a);
        }

        //test with temp var, redo later 
        //var temp = dictionary[floodcam];
        //Debug.Log("-----TEST----- flooding at: " + temp.Name);
        //Debug.Log(temp.Position[0]);
        //FloodCamera(temp);
        //Debug.Log("dictionary.Count);
    }
     
    
    void Update()
    {
        
    }

    private void FloodCamera(CameraData d)
    {
        //Instantiate(daywaterPrefab, new Vector3(d.Position[0], 37.95692f, d.Position[2]), Quaternion.identity);
    }

}
