using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarsPooling : MonoBehaviour
{
    public static CarsPooling sharedInstance;
    public GameObject vehicle1;
    public int amountV1;
    public GameObject vehicle2;
    public int amountV2;
    public GameObject vehicle3;
    public int amountV3;
    public GameObject vehicle4;
    public int amountV4;
    public Vector3 spawnPosition = new Vector3(1000, 1000, 1000);
    public List<GameObject> pool1;
    public List<GameObject> pool2;
    public List<GameObject> pool3;
    public List<GameObject> pool4;

    public void InitPool(List<GameObject> pool, GameObject poolObject, int amount)
    {
        GameObject tmp;
        for (int i = 0; i < amount; i++)
        {
            tmp = Instantiate(poolObject, spawnPosition, Quaternion.identity);
            tmp.SetActive(false);
            pool.Add(tmp);
        }
    }

    public GameObject GetPooledObject(int carID)
    {
        List<GameObject> pool = pool1;
        int amount = amountV1;
        
        if (carID == 1)
        {
            pool = pool2;
            amount = amountV2;
        }
        if (carID == 2)
        {
            pool = pool3;
            amount = amountV3;
        }
        if (carID == 3)
        {
            pool = pool4;
            amount = amountV4;
        }
        for (int i = 0; i < amount; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }

    private void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pool1 = new List<GameObject>();
        InitPool(pool1, vehicle1, amountV1);
        pool2 = new List<GameObject>();
        InitPool(pool2, vehicle2, amountV2);
        pool3 = new List<GameObject>();
        InitPool(pool3, vehicle3, amountV3);
        pool4 = new List<GameObject>();
        InitPool(pool4, vehicle4, amountV4);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
