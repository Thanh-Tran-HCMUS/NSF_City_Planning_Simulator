using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodManager : MonoBehaviour
{
    public Vector3 FloodPosition;
    //public bool cameraOnly = true;// flood at camera location
    public GameObject waterPrefab;

    //private List<GameObject> instantiatedObjects = new List<GameObject>(); // the list of instantiated objects// clone list 

    //public CameraLocator script; //call script
    //private Dictionary<string, CameraData> dictionary = new Dictionary<string, CameraData>();//create dictotionary type
    
    // Start is called before the first frame update
    void Start()
    {
        /*foreach (var a in script.importCamList)
        {
            //dictionary[a.Name] = a;
            dictionary.Add(a.Name, a);
        }

        Debug.Log("Number of camera location saved: "+dictionary.Count);*/
        
        waterPrefab.transform.position = FloodPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpToFloodPosition()
    {
        //FloodPosition.y = Camera.main.transform.position.y;
        //Check if FPS Camera
        if (Camera.main.transform.parent != null)
        {
            Camera.main.transform.parent.GetComponent<CharacterController>().enabled = false;
            FloodPosition.y = Camera.main.transform.parent.position.y + 3f;
            Camera.main.transform.parent.position = FloodPosition;
            Camera.main.transform.parent.GetComponent<CharacterController>().enabled = true;
        }
        else //Check if normal Camera
        {
            FloodPosition.y = Camera.main.transform.position.y;
            Camera.main.transform.position = FloodPosition;
        }
    }

    public void TurnFloodOn()
    {
        waterPrefab.SetActive(true);
        /*if (cameraOnly)
        {
            foreach (KeyValuePair<string, CameraData> kvp in dictionary)
            {
                Debug.Log("Flooding at camera: " + kvp.Key);
                FloodCamera(kvp.Value);
            }
        }*/
    }

    public void TurnFloodOff()
    {
        waterPrefab.SetActive(false);
        /*if (cameraOnly)
        {
            foreach (GameObject flood in instantiatedObjects)
            {
                Destroy(flood);
            }
        }*/
    }

    private void FloodCamera(CameraData d)
    {
        GameObject flood = Instantiate(waterPrefab, new Vector3(d.Position[0], 37.95692f, d.Position[2]), Quaternion.identity);
        //instantiatedObjects.Add(flood);

        //Instantiate(waterPrefab, new Vector3(d.Position[0], 37.95692f, d.Position[2]), Quaternion.identity);
    }

}
