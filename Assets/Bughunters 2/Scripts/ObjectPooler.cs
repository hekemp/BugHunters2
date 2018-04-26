using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    private GameObject CreatePooledObject()
    {
        GameObject obj = Instantiate<GameObject>(objectToPool);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

    // Use this for initialization
    void Start () {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            CreatePooledObject();
        }
	}

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                Debug.Log(" - SAVED");
                return pooledObjects[i];
            }
        }
        return CreatePooledObject();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
