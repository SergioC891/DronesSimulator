using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject droneObjectToPool;
    public int droneAmountToPool;
    public GameObject resourceObjectToPool;
    public int resourceAmountToPool;
    private int totalAmountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();

        putObjectToPool(droneObjectToPool, droneAmountToPool);
        putObjectToPool(resourceObjectToPool, resourceAmountToPool);

        totalAmountToPool = droneAmountToPool + resourceAmountToPool;
    }

    private void putObjectToPool(GameObject objectToPool, int amountToPool)
    {
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject(string objName)
    {
        for (int i = 0; i < totalAmountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].name == objName)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    public bool deactivateLastPooledObject(string objName)
    {
        for (int i = (totalAmountToPool - 1); i >= 0; i--)
        {
            if (pooledObjects[i].activeInHierarchy && pooledObjects[i].name == objName)
            {
                pooledObjects[i].SetActive(false);
                return true;
            }
        }

        return false;
    }
}