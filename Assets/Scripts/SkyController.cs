using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public delegate void changedaynight();

public class SkyController : MonoBehaviour
{
    public Material nightSky;
    public GameObject DirectionLightNight;
    public Material daySky;
    public GameObject DirectionLightDay;
    // Start is called before the first frame update
    private static bool day = true;
    public static changedaynight changedaynighthandler = null;

    public static bool DayNight
    {
        get => day;
        set
        {
            day = value;
            changedaynighthandler?.Invoke();
        }
    }

    void Start()
    {
        if (day)
        {
            ChangeDayTime();
        }
        else
            ChangeNightTime();
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeNightTime()
    {
        RenderSettings.skybox = nightSky;
        RenderSettings.ambientLight = Color.black;
        DirectionLightNight.SetActive(true);
        DirectionLightDay.SetActive(false);
        GameObject[] lights = GameObject.FindGameObjectsWithTag("SpotStreetLight");
        foreach (GameObject light in lights)
        {
            light.transform.GetComponent<Light>().enabled = true;
            light.transform.GetComponent<VolumetricLightBeam>().enabled = true;
        }
        day = false;
    }

    public void ChangeDayTime()
    {
        RenderSettings.skybox = daySky;
        RenderSettings.ambientLight = Color.white;
        DirectionLightNight.SetActive(false);
        DirectionLightDay.SetActive(true);
        GameObject[] lights = GameObject.FindGameObjectsWithTag("SpotStreetLight");
        foreach (GameObject light in lights)
        {
            light.transform.GetComponent<Light>().enabled = false;
            light.transform.GetComponent<VolumetricLightBeam>().enabled = false;
        }
        day = true;
    }
}
