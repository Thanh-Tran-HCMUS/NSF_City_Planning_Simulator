using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursorController : MonoBehaviour
{
    public GameObject VisualOff;
    private void OnEnable()
    {
        Cursor.visible = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Cursor.visible = !Cursor.visible;
            Cursor.visible = true;
            VisualOff.GetComponent<Button>().onClick.Invoke();
        }
    }
}
