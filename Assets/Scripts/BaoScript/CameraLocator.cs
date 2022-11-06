using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CameraLocator : MonoBehaviour
{
    private CameraData cameraData; 
    public GameObject [] streetCamera;
    private string filePath;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Tag camera with StreetCam, then press C to save location");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            streetCamera = GameObject.FindGameObjectsWithTag("StreetCam");
            //filePath = Application.persistentDataPath + "CameraLocation.json";
            filePath = Application.dataPath + "/CameraLocation.json";
            if (File.Exists(filePath)){
                File.Delete(filePath);
                Debug.Log("Camera Location Replaced");
                UnityEditor.AssetDatabase.Refresh();
            }

            if (streetCamera.Length == 0)
            {
                Debug.Log("No camera found!");
            }

            foreach (GameObject g in streetCamera)
            {
                CameraData camSave = new CameraData(g);
                SaveData(camSave);

            }
        }
    
    }
    
    public void SaveData(CameraData c)
    {
        string savePath = filePath;
        Debug.Log("Saving Data at" + savePath);
        
        string json = JsonUtility.ToJson(c);
        Debug.Log(json);

        using (StreamWriter sw = new StreamWriter(savePath, true))
        {
            sw.WriteLine(json);
        }
    }

}
