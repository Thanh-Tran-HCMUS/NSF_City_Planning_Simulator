using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class LoadDataFromServer : MonoBehaviour
{
    List<List<int>> dataLines = new List<List<int>>();
    List<List<List<int>>> dataList = new List<List<List<int>>>();
    PathFinder pathFinder;
    public GameObject pathFinderObj;
    public enum ListCity { District1, ThuDuc};
    public ListCity City;
    public Slider speedSlider;
    private string[] txtFileName;
    string[] cityName = { "District1/", "ThuDuc/" };
    string[] D1File = { "96.txt", "97.txt", "99.txt", "102.txt" };
    string[] TDFile = { "45.txt", "52.txt", "56.txt", "68.txt", "71.txt", "72.txt", "74.txt", "79.txt", "81.txt" };
    int currentFile = 0;
    //List<string> files = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = pathFinderObj.GetComponent<PathFinder>();
        //TextFilesServer(get_City());
        listFileTxt(get_City());
        GetDataFromServer();
        //StartCoroutine(dataSpawn(dataList));
    }
    private void listFileTxt(string name)
    {
        if (name == "District1")
        {
            txtFileName = new string[]{"96", "97", "99", "102"};
        }
        else if (name == "ThuDuc")
        {
            txtFileName = new string[] { "45", "52", "56", "68", "71", "72", "74", "79","81" };
        }
    }
    private string get_City()
    {
        string name = "";
        switch (City)
        {
            case ListCity.District1:
                name = "District1";
                break;
            case ListCity.ThuDuc:
                name = "ThuDuc";
                break;
        }
        return name;
    }
    /*
    public void TextFilesServer(string name)
    {
       
        if (name == "District1")
        {
            TextFileServer(name + "/96.txt");
            TextFileServer(name + "/97.txt");
            TextFileServer(name + "/99.txt");
            TextFileServer(name + "/102.txt");
        }
        else if (name == "ThuDuc")
        {
            Debug.Log("Vao day nhe");
            TextFileServer(name + "/45.txt");
            TextFileServer(name + "/52.txt");
            TextFileServer(name + "/56.txt");
            TextFileServer(name + "/68.txt");
            TextFileServer(name + "/71.txt");
            TextFileServer(name + "/72.txt");
            TextFileServer(name + "/74.txt");
            TextFileServer(name + "/79.txt");
            TextFileServer(name + "/81.txt");
        }
    }*/
    public void GetDataFromServer()
    {
        if (City == ListCity.District1)
        {
            TextFileServer(cityName[0] + D1File[currentFile]);
        }
        else //Thu Duc
        {
            TextFileServer(cityName[1] + TDFile[currentFile]);
        }
    }
    private void TextFileServer(string name)
    {
        StartCoroutine(GetData(name));
    }
    IEnumerator GetData(string filename)
    {
        List<int> data = new List<int>();
        string url = "https://csl-hcmc.com/data/" + filename;

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log("Vao day nhe 01");
            //Debug.Log(www.downloadHandler.text);
            string tmp = www.downloadHandler.text;
            var lines = tmp.Split('\n');
            foreach (var line in lines)
            {
                var values = line.Split(' ');
                foreach (var value in values)
                {
                    int val = 0;
                    if (int.TryParse(value, out val))
                        data.Add(val);
                }
                dataLines.Add(data);
            }
            dataList.Add(dataLines);
            currentFile++;
            if (City == ListCity.District1)
            {
                if (currentFile < D1File.Count())
                    TextFileServer(cityName[0] + D1File[currentFile]);
                else
                {
                    StartCoroutine(dataSpawn(dataList));
                }
            }
            else //Thu Duc
            {
                if (currentFile < TDFile.Count())
                    TextFileServer(cityName[1] + TDFile[currentFile]);
                else
                {
                    StartCoroutine(dataSpawn(dataList));
                }
            }
        }
    }
    private void Update()
    {
        
    }
    IEnumerator dataSpawn(List<List<List<int>>> textfiles)
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            for (int i = 0; i < 120; i++)
            {
                //Debug.Log("textfiles.Count: " + textfiles.Count);
                for (int j = 0; j < textfiles.Count; j++)
                {
                    string nameStr = txtFileName[j];
                    //Debug.Log(textfiles[j][i][0]);
                    //Debug.Log(textfiles[j][i][1]);
                    //Debug.Log(textfiles[j][i][2]);
                    //Debug.Log(textfiles[j][i][3]);

                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][0]/3, nameStr, 0));
                    yield return new WaitForSeconds(1);
                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][1]/3, nameStr, 1));
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][2]/3, nameStr, 2));
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][3]/3, nameStr, 3));
                    yield return new WaitForSeconds(15f);
                }
                //Debug.Log("Batdaudoi");
                int waitTime = (int)(60 / speedSlider.value);
                yield return new WaitForSeconds(waitTime);
                //Debug.Log("Doixong");
            }
        }  
    }
   
    void ParseFile(string path)
    {
        //Debug.Log("parseFile");
        char[] separators = { ',', ';', '|',' ' };
        string[] strValues = path.Split(separators);
        List<int> intValues = new List<int>();
        foreach (string str in strValues)
        {
            int val = 0;
            if (int.TryParse(str, out val))
                intValues.Add(val);
        }
        dataLines.Add(intValues);
    }
    void printData()
    {
        //Debug.Log(dataList.Count);
        for (int i = 0; i < dataList.Count; i++)
        {
            Debug.Log(dataList[i].Count);
            for (int j = 0; j < dataLines.Count; j++)
            {
                for (int k = 0; k < dataLines[j].Count; k++)
                {
                    Debug.Log("DataList "+i +" : "+dataLines[j][k]);
                }
            }
        }
    }
   
}
