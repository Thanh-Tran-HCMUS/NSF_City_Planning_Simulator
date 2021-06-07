using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class Actor : MonoBehaviour
{
    public ActorData data = new ActorData();
    public string name = "Bridge";
    public float health = 100; 
    public void StoreData()
    {
        Vector3 pos = transform.position;
        data.name = name;
        data.posX = pos.x;
        data.posY = pos.y;
        data.posZ = pos.z;
        data.health = health;

    }

    public void LoadData()
    {
        name = data.name;
        transform.position = new Vector3(data.posX, data.posY, data.posZ);
        health = data.health;

    }

    void OnEnable()
    {
        SaveData.OnLoaded += delegate { LoadData(); };
        SaveData.OnBeforeSave += delegate { StoreData(); };
        SaveData.OnBeforeSave += delegate { SaveData.AddActorData(data); };
    }

    void OnDisable()
    {
        SaveData.OnLoaded -= delegate { LoadData(); };
        SaveData.OnBeforeSave -= delegate { StoreData(); };
        SaveData.OnBeforeSave -= delegate { SaveData.AddActorData(data); };
    }
}

public class ActorData
{
    [XmlAttribute("Name")]
    public string name;

    [XmlAttribute("PosX")]
    public float posX;

    [XmlAttribute("PosY")]
    public float posY;

    [XmlAttribute("PosZ")]
    public float posZ;

    [XmlAttribute("Health")]
    public float health;


}
