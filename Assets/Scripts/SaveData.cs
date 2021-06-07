using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveData
{
    public static ActorContainer actorContainer = new ActorContainer();
    public delegate void SerializeAction();
    public static event SerializeAction OnLoaded;
    public static event SerializeAction OnBeforeSave;

    public static void Load(string path)
    {
        actorContainer = LoadActors(path);
        foreach (ActorData data in actorContainer.actors)
        {
            GameController.CreateActor(data, GameController.playerPath, new Vector3(data.posX, data.posY, data.posZ), Quaternion.identity);

        }
        OnLoaded();

    }

    public static void Save(string path, ActorContainer actors)
    {
        OnBeforeSave();
        SaveActors(path, actors);
        ClearActors();

    }

    public static void AddActorData(ActorData data)
    {
        actorContainer.actors.Add(data);
    }

    public static void ClearActors()
    {
        actorContainer.actors.Clear();
    }

    private static ActorContainer LoadActors(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ActorContainer));
        FileStream stream = new FileStream(path, FileMode.Open);
        ActorContainer actors = serializer.Deserialize(stream) as ActorContainer;
        stream.Close();
        return actors;
        }

    private static void SaveActors(string path, ActorContainer actors)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ActorContainer));
        FileStream stream = new FileStream(path, FileMode.Truncate);
        serializer.Serialize(stream, actors);
        stream.Close();
    }
    
}
