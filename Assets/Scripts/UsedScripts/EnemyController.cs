using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private ObjectPool pool;

    public void SetPool(ObjectPool objectPool)
    {
        pool = objectPool;
    }

    public void Die()
    {
        if (pool != null)
        {
            pool.ReturnObject(this.gameObject);
        }
        else
        {
            Debug.LogWarning("No pool assigned. Destroying instead.");
            Destroy(gameObject);
        }
    }
}