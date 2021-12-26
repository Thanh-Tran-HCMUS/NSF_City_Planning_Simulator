using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class UICaller : MonoBehaviour
{
    public static String CurrentScene; 
    public event EventHandler OnMKeyPressed;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            CurrentScene = SceneManager.GetActiveScene().name;
            OnMKeyPressed?.Invoke(this, EventArgs.Empty); //if not 
            LoadRoadEditor();
            Debug.Log(CurrentScene);
        }
    }

    private void LoadRoadEditor()
    {
        SceneManager.LoadScene("RoadEditorPreset",LoadSceneMode.Single); // aditive might crash 
    }
}
