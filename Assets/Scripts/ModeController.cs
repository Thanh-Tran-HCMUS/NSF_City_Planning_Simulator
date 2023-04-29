using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeController : MonoBehaviour
{

    public void ChangeToDesignMode(string designScene)
    {
        SceneManager.LoadScene(designScene);
    }

    public void ChangeToVisualizationMode(string viewScene)
    {
        SceneManager.LoadScene(viewScene);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
