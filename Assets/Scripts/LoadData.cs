using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

public class Values : Collection<int>
{
    public Values(string line)
    {
        var values = line.Split(' ');
        foreach (var value in values)
            Add(int.Parse(value));
    }
}

public class TextFile : Collection<Values>
{
    public TextFile(string filename)
    {
        var lines = File.ReadAllLines(filename);
        foreach (var line in lines)
            Add(new Values(line));
    }
}

public class TextFiles : Collection<TextFile>
{
    public TextFiles(string folderName)
    {
        var files = Directory.GetFiles(folderName, "*.txt");
        foreach (var file in files)
            Add(new TextFile(file));
    }
}
public class LoadData : MonoBehaviour
{
    //public Street street;
    //public Street streetleaving;
    List<List<int>> dataLines = new List<List<int>>();
    List<List<List<int>>> dataList = new List<List<List<int>>>();
    PathFinder pathFinder;
    public GameObject pathFinderObj;
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = pathFinderObj.GetComponent<PathFinder>();
        string myPath = Application.dataPath + "/Data/";
        var files = new TextFiles(myPath);
        StartCoroutine(dataSpawn(files));
        //Debug.Log("data ne: " + files[0][1][1]);
        //Debug.Log(files.Count);
        //StartCoroutine(importData("45.Duong14"));
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            foreach (Street b in pathFinder.graphData.allStreets)
            {
                if (b.Name == "hvt1")
                {
                    Debug.Log(b.center.ID);
                }
            }
            //foreach (Node a in graphData.centers)
            //{
            //    Debug.Log(a.pathDistance + "   :    " + a.ID);

            //}
            // SpawnRealtime(13);
        }
        if (Input.GetKey(KeyCode.C))
        {
            string myPath = Application.dataPath + "/Data/";
            DirectoryInfo dir = new DirectoryInfo(myPath);
            FileInfo[] info = dir.GetFiles("*.txt");
            foreach (FileInfo f in info)
            {
                List<string> fileLines = File.ReadAllLines(myPath+f.Name).ToList(); 
                foreach (string line in fileLines)
                {
                    ParseFile(line);
                }
                dataList.Add(dataLines);
               // Debug.Log("DataLines count: "+dataLines.Count);
                dataLines.RemoveRange(0,dataLines.Count);
                //Debug.Log("Data Line Remove: " + dataLines.Count);
            }
        }
        if (Input.GetKey(KeyCode.O))
        {
            string myPath = Application.dataPath + "/Data/";
            var files = new TextFiles(myPath);
            Debug.Log("data ne: "+files[0][1][1]);
            Debug.Log(files.Count);

        }
    }
    IEnumerator importData(string nameStr)
    {
        StartCoroutine(pathFinder.Spawn(dataLines[0][0], nameStr, 0));
        StartCoroutine(pathFinder.Spawn(dataLines[0][1], nameStr, 1));
        StartCoroutine(pathFinder.Spawn(dataLines[0][2], nameStr, 2));
        yield return new WaitForSeconds(6);
        StartCoroutine(pathFinder.Spawn(dataLines[0][3], nameStr, 3));

        // pathFinder.spawCarInStreet("hvt", "hvt2", 0);
        // street.spawns[0] = dataLines[0][2];
    }
    IEnumerator dataSpawn(TextFiles textfiles)
    {
       for(int i = 0; i < 120; i++)
        {
            for(int j = 0; j < textfiles.Count; j++)
            {
                string nameStr = NameStreet(j);
                StartCoroutine(pathFinder.Spawn(textfiles[j][i][0], nameStr, 0));
                StartCoroutine(pathFinder.Spawn(textfiles[j][i][1], nameStr, 1));
                StartCoroutine(pathFinder.Spawn(textfiles[j][i][2], nameStr, 2));
                StartCoroutine(pathFinder.Spawn(textfiles[j][i][3], nameStr, 3));
                //Debug.Log(NameStreet(j));
                //Debug.Log("textfiles[" + j + "][" + i + "][0]: " + textfiles[j][i][0]);
                //Debug.Log("textfiles[" + j + "][" + i + "][1]: " + textfiles[j][i][1]);
                //Debug.Log("textfiles[" + j + "][" + i + "][2]: " + textfiles[j][i][2]);
                //Debug.Log("textfiles[" + j + "][" + i + "][3]: " + textfiles[j][i][3]);
            }
            Debug.Log("Batdaudoi");
            yield return new WaitForSeconds(60);
            Debug.Log("Doixong");
        }      
    }
   
    string NameStreet(int i)
    {
        string name = "";
        switch(i)
        {
            case 0:
                name = "45.Duong14";
                break;
            case 1:
                name = "52.GanDuong13";
                break;
            case 2:
                name = "56.DauDuong13";
                break;
            case 3:
                name = "68.DuongE";
                break;
            case 4:
                name = "71.PhamVanDong";
                break;
            case 5:
                name = "72.14Giao13";
                break;
            case 6:
                name = "74.DuongSo2";
                break;
        }
        return name;
    }
    void ParseFile(string path)
    {
        Debug.Log("parseFile");
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
        //for(int i = 0; i < dataLines.Count; i++)
        //{
        //    for(int j = 0; j < dataLines[i].Count; j++)
        //    {
        //        Debug.Log(dataLines[i][j]);
        //    }
        //}
        Debug.Log(dataList.Count);
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
