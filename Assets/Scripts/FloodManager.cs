using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodManager : MonoBehaviour
{
    public Vector3 FloodPosition;
    public bool cameraOnly = true;// flood at camera location
    public GameObject waterPrefab;

    private List<GameObject> instantiatedObjects = new List<GameObject>(); // the list of instantiated objects// clone list 

    public CameraLocator script; //call script
    private Dictionary<string, CameraData> dictionary = new Dictionary<string, CameraData>();//create dictotionary type
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var a in script.importCamList)
        {
            //dictionary[a.Name] = a;
            dictionary.Add(a.Name, a);
        }

        Debug.Log("Number of camera location saved: "+dictionary.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpToFloodPosition()
    {
        FloodPosition.y = Camera.main.transform.position.y;
        Camera.main.transform.position = FloodPosition;
    }

    public void TurnFloodOn()
    {
        if (cameraOnly)
        {
            foreach (KeyValuePair<string, CameraData> kvp in dictionary)
            {
                Debug.Log("Flooding at camera: " + kvp.Key);
                FloodCamera(kvp.Value);
            }
        }
    }

    public void TurnFloodOff()
    {
        if (cameraOnly)
        {
            foreach (GameObject flood in instantiatedObjects)
            {
                Destroy(flood);
            }
        }
    }

    private void FloodCamera(CameraData d)
    {
        GameObject flood = Instantiate(waterPrefab, new Vector3(d.Position[0], 37.95692f, d.Position[2]), Quaternion.identity);
        instantiatedObjects.Add(flood);

        //Instantiate(waterPrefab, new Vector3(d.Position[0], 37.95692f, d.Position[2]), Quaternion.identity);
    }

}
