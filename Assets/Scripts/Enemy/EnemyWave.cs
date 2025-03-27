using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int enemiesPerWave = 5;
    public float waveDuration = 10f;
    public float spawnInterval = 1.5f;

    [Header("Spawn Area")]
    public float minX = -5f;
    public float maxX = 5f;
    public float spawnY = 0f;
    public float spawnZ = 0f;

    private bool isSpawning = false;
    private WaveManager waveManager;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>(); // Find the WaveManager in the scene
    }

    public void StartWave()
    {
        if (!isSpawning)
        {
            StartCoroutine(WaveSequence());
        }
    }

    private IEnumerator WaveSequence()
    {
        isSpawning = true;

        // Announce new wave
        if (waveManager != null)
        {
            waveManager.AnnounceWave();
            yield return new WaitForSeconds(waveManager.announcementDuration + 1f); // Wait for announcement
        }

        float startTime = Time.time;
        int enemiesSpawned = 0;

        while (Time.time < startTime + waveDuration && enemiesSpawned < enemiesPerWave)
        {
            SpawnEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // Increase wave number for the next round
        if (waveManager != null)
        {
            waveManager.IncreaseWave();
        }

        isSpawning = false;
    }

    private void SpawnEnemy()
    {
        float spawnX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}

