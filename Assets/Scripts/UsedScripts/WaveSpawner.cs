using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    public List<Enemy> enemies = new List<Enemy>();
    public int currWave;
    private int waveValue;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();

    public Transform[] spawnLocation;
    public int spawnIndex;

    public int waveDuration;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;

    public List<GameObject> spawnedEnemies = new List<GameObject>();

    public ObjectPool enemyPoolPrefab; // Create different pools for each enemy type if needed
    public int poolSize = 10; 
    private Dictionary<string, ObjectPool> enemyPools;
    // Start is called before the first frame update
    void Start()
    {
        GenerateWave();
        enemyPools = new Dictionary<string, ObjectPool>();

        foreach (Enemy e in enemies)
        {
            // Initialize object pools for each enemy type
            ObjectPool pool = new GameObject(e.enemyPrefab.name + "_Pool").AddComponent<ObjectPool>();
            pool.prefab = e.enemyPrefab;
            pool.poolSize = poolSize;
            pool.transform.SetParent(this.transform);
            enemyPools[e.enemyPrefab.name] = pool;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        spawnedEnemies.RemoveAll(item => item == null);

        //Debug.Log(spawnedEnemies.Count);
        if (spawnTimer <= 0)  
        {          
            //spawn an enemy
            if (enemiesToSpawn.Count > 0)
            {
                GameObject prefab = enemiesToSpawn[0];
                if (enemyPools.TryGetValue(prefab.name, out ObjectPool pool))
                {
                    GameObject enemy = pool.GetObject();
                    if (enemy != null)
                   {
                   enemy.transform.position = spawnLocation[spawnIndex].position;
                   enemy.transform.rotation = Quaternion.identity;
                   
                   // Assign this pool to the enemy, so it can return itself to the pool when it dies
                   EnemyController enemyController = enemy.GetComponent<EnemyController>();
                   if (enemyController != null)
                   {
                       enemyController.SetPool(pool);
                   }
                   enemy.SetActive(true);
                   spawnedEnemies.Add(enemy);
                   }
              else
               {
                Debug.LogWarning("No pooled enemy available for " + prefab.name);
               }
                enemiesToSpawn.RemoveAt(0);
                spawnedEnemies.Add(enemy);
                spawnTimer = spawnInterval;

                if (spawnIndex + 1 <= spawnLocation.Length - 1)
                {
                    spawnIndex++;
                }
                else
                {
                    spawnIndex = 0;
                }
            }
            else
            {
                Debug.Log("HIERRRRRR");
                waveTimer = 0; // if no enemies remain, end wave
            }
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }

        if (waveTimer <= 0) // && spawnedEnemies.Count <= 0)
        {
            currWave++;
            GenerateWave();
        }
    }
}

    public void GenerateWave()
    {
        waveValue = currWave * 10;
        GenerateEnemies();

        spawnInterval = waveDuration / enemiesToSpawn.Count; // gives a fixed time between each enemies
        waveTimer = waveDuration; // wave duration is read only
    }

    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate
        // 
        // in a loop grab a random enemy 
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.

        // repeat... 

        //  -> if we have no points left, leave the loop

        List<GameObject> generatedEnemies = new List<GameObject>();
        int safety = 1000; // prevent infinite loop
        
        while (waveValue > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int cost;
}

