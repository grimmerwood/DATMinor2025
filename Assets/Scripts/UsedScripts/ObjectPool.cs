using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize = 10;
    private Queue<GameObject> pool = new Queue<GameObject>();

    // Manual initializer (call from WaveSpawner)
    public void Initialize()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = prefab.name; // ensure name matches for lookup
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        // Optional: Instantiate more if none available
        GameObject newObj = Instantiate(prefab);
        newObj.name = prefab.name;
        newObj.SetActive(false);
        pool.Enqueue(newObj);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
