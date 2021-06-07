using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    public const string playerPath = "Prefabs/Bridge";
    private static string dataPath = string.Empty;

    void Awake()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            dataPath = System.IO.Path.Combine(Application.persistentDataPath, "Resources/actors.xml");
        else
            dataPath = System.IO.Path.Combine(Application.dataPath, "Resources/actors.xml");
        //add something into the map
      
        
        }

    void Start()
    {
        Debug.Log("We are doing something");
        // CreateActor(playerPath, new Vector3(0, 4.712296f,0), Quaternion.identity);
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 1.5f, 0);

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0, 1.5f, 0);

        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.transform.position = new Vector3(2, 1.5f, 0);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = new Vector3(-2, 3f, 0);
    }
    public static Actor CreateActor(string path, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        GameObject go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
        Actor actor = go.GetComponent<Actor>() ?? go.AddComponent< Actor>();
        return actor;
    }

    public static Actor CreateActor(ActorData data, string path, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        GameObject go = GameObject.Instantiate(prefab, position, rotation) as GameObject;
        Actor actor = go.GetComponent<Actor>() ?? go.AddComponent<Actor>();
        actor.data = data;
        return actor;
    }

    void OnEnable()
    {
        saveButton.onClick.AddListener(delegate { SaveData.Save(dataPath, SaveData.actorContainer); });
        loadButton.onClick.AddListener(delegate { SaveData.Load(dataPath); });
    }

    void OnDisable()
    {
        saveButton.onClick.RemoveListener(delegate { SaveData.Save(dataPath, SaveData.actorContainer); });
        loadButton.onClick.RemoveListener(delegate { SaveData.Load(dataPath); });
    }
}
