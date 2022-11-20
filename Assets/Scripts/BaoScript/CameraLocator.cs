using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq; 

[Serializable]
public class CameraLocator : MonoBehaviour
{
    private CameraData cameraData;  // call class object
    public GameObject[] streetCamera;
    private string filePath;
    string filename = "cameralocation.json";

    public List<CameraData> camList = new List<CameraData>(); // class object list 

    public void AddDataToLIST(GameObject g) // method to add 
    {
        camList.Add(new CameraData(g));
        ToJSON<CameraData>(camList, filename);// classify type and give list 
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Tag camera with StreetCam, then press C to save location");
        camList = FromJSON<CameraData>(filename);
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Data store to: "+GetPath(filename));
            streetCamera = GameObject.FindGameObjectsWithTag("StreetCam");

            if (streetCamera.Length == 0)
            {
                Debug.Log("No camera found!");
            }

            foreach (GameObject g in streetCamera)
            {
                AddDataToLIST(g);
            }
        }
    }
    
    public void ToJSON<T>(List<T> toSave, string filename)//covert object to json format, generic <T>
    {
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteData(GetPath(filename), content);
    }

    public List<T> FromJSON<T>(string filename){
        string content = ReadData(GetPath(filename));
        if(string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> result = JsonHelper.FromJson<T>(content).ToList();

        return result;
    }

    private string GetPath(string filename)
    {
        return Application.persistentDataPath + "/" + filename;
        //filePath = Application.persistentDataPath + "CameraLocation.json"; for oculus use
        //filePath = Application.dataPath + "/CameraLocation.json"; for test use
    }

    public void WriteData(string path, string content)// auto write and detect content
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter sw = new StreamWriter(fileStream))
        {
            sw.Write(content);
        }
    }

    public string ReadData(string path)
    {
        if (File.Exists(path))
        {
            using(StreamReader sr = new StreamReader(path))
            {
                string content = sr.ReadToEnd();
                return content; 
            }
        }
        return "";
    }

    public static class JsonHelper //json wrapper > list
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Cameras;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Cameras = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Cameras = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Cameras;
        }
    }
}