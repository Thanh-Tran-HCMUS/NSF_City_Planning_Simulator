using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodSystem : MonoBehaviour
{
    [SerializeField] GameObject daywaterPrefab;
    [SerializeField] GameObject nightwaterPrefab;

    public string floodcam;// test flood location

    public CameraLocator script; //call script
    Dictionary<string, CameraData> dictionary = new Dictionary<string, CameraData>();//create dictotionary type

    
    void Start()
    {
        foreach(var a in script.importCamList)
        {
            //dictionary[a.Name] = a;
            dictionary.Add(a.Name, a);
        }

        var temp = dictionary[floodcam];
        Debug.Log("-----TEST----- flooding at: " + temp.Name);
        //Debug.Log(temp.Position[0]);
        FloodSpawner(temp);
    }
     
    
    void Update()
    {
        
    }

    private void FloodSpawner(CameraData d)
    {
        Instantiate(daywaterPrefab, new Vector3(d.Position[0], 37.95692f, d.Position[2]), Quaternion.identity);
    }
}
