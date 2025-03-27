using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerWave : MonoBehaviour
{
    public Wave[] waves;

    private Wave currentWave;

    [System.Serializable]
    public class Wave
    {
        public GameObject[] EnemiesInWave;  // Array of enemies that will spawn in this wave
        public int NumberToSpawn;           // The number of enemies to spawn in the wave
        public float TimeBeforeThisWave;    // The time before the next wave starts
    }


    [SerializeField]
    private Transform[] spawnpoints;

    private float timeBtwnSpawns;
    private int i = 0;

    private bool stopSpawning = false;

    private void Awake()
    {

        currentWave = waves[i];
        timeBtwnSpawns = currentWave.TimeBeforeThisWave;
    }

    private void Update()
    {
        if (stopSpawning)
        {
            return;
        }

        if (Time.time >= timeBtwnSpawns)
        {
            SpawnWave();
            IncWave();

            timeBtwnSpawns = Time.time + currentWave.TimeBeforeThisWave;
        }
    }

    private void SpawnWave()
    {
        for (int i = 0; i < currentWave.NumberToSpawn; i++)
        {
            int num = Random.Range(0, currentWave.EnemiesInWave.Length);
            int num2 = Random.Range(0, spawnpoints.Length);

            Instantiate(currentWave.EnemiesInWave[num], spawnpoints[num2].position, spawnpoints[num2].rotation);
        }
    }

    private void IncWave()
    {
        if (i + 1 < waves.Length)
        {
            i++;
            currentWave = waves[i];
        }
        else
        {
            stopSpawning = true;
        }
    }
}