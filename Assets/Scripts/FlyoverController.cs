using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyoverController : MonoBehaviour
{
    public GameObject Junction;
    public GameObject Street1;
    public GameObject Street2;

    public void TurnOnFlyover()
    {
        Junction.SetActive(true);
        Street1.SetActive(true);
        Street2.SetActive(true);
        List<Path> streetPath = Street1.GetComponent<Street>().paths;
        for (int i = 0; i < streetPath.Count; i++)
        {
            streetPath[i].Cost = 0;
        }
        streetPath = Street2.GetComponent<Street>().paths;
        for (int i = 0; i < streetPath.Count; i++)
        {
            streetPath[i].Cost = 0;
        }
    }

    public void TurnOffFlyover()
    {
        Junction.SetActive(false);
        Street1.SetActive(false);
        Street2.SetActive(false);
        List<Path> streetPath = Street1.GetComponent<Street>().paths;
        for (int i = 0; i < streetPath.Count; i++)
        {
            streetPath[i].Cost = 1000;
        }
        streetPath = Street2.GetComponent<Street>().paths;
        for (int i = 0; i < streetPath.Count; i++)
        {
            streetPath[i].Cost = 1000;
        }
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
