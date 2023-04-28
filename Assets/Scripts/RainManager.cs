using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    public GameObject Rain;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnRainOn()
    {
        Rain.SetActive(true);
    }

    public void TurnRainOff()
    {
        Rain.SetActive(false);
    }
}
