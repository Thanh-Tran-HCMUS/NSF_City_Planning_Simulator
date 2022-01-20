using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    //public GameObject menu;
    private void Start()
    {
        //menu.SetActive(false);
    }
    // Start is called before the first frame update
    string nameScene;
    public void LoadSceneFromName(string name)
    {
        SceneManager.LoadSceneAsync(name);
        Debug.Log("ClickLoadScene");
    }
    public void LoadSceneNoAsync(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void ClickLoadScene() {
        Debug.Log("ClickLoadScene");
    }
    public void ClickSymbol(string name)
    {
        nameScene = name;
        //menu.SetActive(true);
    }
    public void ClickOK()
    {
        //menu.SetActive(false);
        //SceneManager.UnloadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadSceneAsync(nameScene);
    }
    public void ClickCancel()
    {
        //menu.SetActive(false);
    }
}
