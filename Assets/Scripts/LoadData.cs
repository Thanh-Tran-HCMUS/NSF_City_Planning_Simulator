using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    List<List<int>> dataLines = new List<List<int>>();
    List<List<List<int>>> dataList = new List<List<List<int>>>();
    PathFinder pathFinder;
    public GameObject pathFinderObj;
    public enum ListCity { District1, ThuDuc};
    public ListCity City;
    public Slider speedSlider;
    private string[] txtFileName;

    // Start is called before the first frame update
    void Start()
    {
        DataByDate.changehandler = changeDate;
        pathFinder = pathFinderObj.GetComponent<PathFinder>();
        string myPath;
#if UNITY_ANDROID
        myPath = Application.streamingAssetsPath + "/Data/"+get_City()+"/"+ DataByDate.Date+"/";
#else
        myPath = Application.dataPath + "/Data/"+get_City()+"/"+ DataByDate.Date+"/";
#endif
        Debug.Log(myPath);
        var files = new TextFiles(myPath);
        listFileTxt(myPath);
        StartCoroutine(dataSpawn(files));
    }
    public void changeDate()
    {
        string myPath;
#if UNITY_ANDROID
        myPath = Application.streamingAssetsPath + "/Data/"+get_City()+"/"+ DataByDate.Date+"/";
#else
        myPath = Application.dataPath + "/Data/" + get_City() + "/" + DataByDate.Date + "/";
#endif
        Debug.Log(myPath);
        var files = new TextFiles(myPath);
        listFileTxt(myPath);
        StartCoroutine(dataSpawn(files));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//reset current scene#bao
        Debug.Log("Change date then reset scene");// debug#bao
    }
    private void listFileTxt(string myPath)
    {
        DirectoryInfo dir = new DirectoryInfo(myPath);
        FileInfo[] info = dir.GetFiles("*.txt");
        txtFileName = new string[info.Length];
        Debug.Log(txtFileName.Length);
        int i = 0;
        foreach (FileInfo f in info)
        {
            txtFileName[i] = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
            i++;
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
    private void Update()
    {
        /*if (Input.GetKey(KeyCode.B))
        {
            foreach (Street b in pathFinder.graphData.allStreets)
            {
                if (b.Name == "hvt1")
                {
                    Debug.Log(b.center.ID);
                }
            }
        }
        if (Input.GetKey(KeyCode.C))
        {
            string myPath = Application.dataPath + "/Data/ThuDuc/";
            DirectoryInfo dir = new DirectoryInfo(myPath);
            FileInfo[] info = dir.GetFiles("*.txt");
            txtFileName = new string[info.Length];
            Debug.Log(txtFileName.Length);
            int i = 0;
            foreach (FileInfo f in info)
            {
                txtFileName[i] = f.Name.Substring(0, f.Name.Length - f.Extension.Length);
                Debug.Log("name: " + f.Name.Substring(0,f.Name.Length-f.Extension.Length));
                i++;
               // List<string> fileLines = File.ReadAllLines(myPath+f.Name).ToList(); 
               // foreach (string line in fileLines)
               // {
               //     ParseFile(line);
               // }
               // dataList.Add(dataLines);
               //// Debug.Log("DataLines count: "+dataLines.Count);
               // dataLines.RemoveRange(0,dataLines.Count);
               // //Debug.Log("Data Line Remove: " + dataLines.Count);
            }
            for(int k = 0; k < txtFileName.Length; k++)
            {
                Debug.Log("txtfilenane: "+txtFileName[k]);
            }
        }
        if (Input.GetKey(KeyCode.O))
        {
            for (int k = 0; k < txtFileName.Length; k++)
            {
                Debug.Log("txtfilenane: " + txtFileName[k]);
            }
        }*/
    }
    IEnumerator dataSpawn(TextFiles textfiles)
    {
        while (true)
        {
            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < textfiles.Count; j++)
                {
                    string nameStr = txtFileName[j];
                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][0]/3, nameStr, 0));
                    yield return new WaitForSeconds(1);
                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][1]/3, nameStr, 1));
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][2]/3, nameStr, 2));
                    yield return new WaitForSeconds(1f);
                    StartCoroutine(pathFinder.Spawn(textfiles[j][i][3]/3, nameStr, 3));
                    yield return new WaitForSeconds(15f);
                }
                Debug.Log("Batdaudoi");
                int waitTime = (int)(60 / speedSlider.value);
                yield return new WaitForSeconds(waitTime);
                Debug.Log("Doixong");
            }
        }  
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
