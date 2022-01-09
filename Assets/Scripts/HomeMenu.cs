using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
 
    public void load_ThuDucCity()
    {
        SceneManager.LoadSceneAsync("ThuDucCity");   
    }

    public void load_District1()
    {
        SceneManager.LoadSceneAsync("District1");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
