using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodManager : MonoBehaviour
{
    public Vector3 FloodPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpToFloodPosition()
    {
        FloodPosition.y = Camera.main.transform.position.y;
        Camera.main.transform.position = FloodPosition;
    }

    public void TurnFloodOn()
    {

    }

    public void TurnFloodOff()
    {

    }
}
