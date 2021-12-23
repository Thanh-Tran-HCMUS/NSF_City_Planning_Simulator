using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void LoadSceneFromName(string name)
    {
        SceneManager.LoadScene(name);
        Debug.Log("ClickLoadScene");
    }
    public void ClickLoadScene() {
        Debug.Log("ClickLoadScene");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
