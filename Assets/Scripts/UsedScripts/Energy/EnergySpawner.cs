using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySpawner : MonoBehaviour
{
    [Header("Spawn Area (use 2 empty GameObjects)")]
    public Transform spawnAreaMin;
    public Transform spawnAreaMax;

    [Header("Energy Settings")]
    public GameObject energyPrefab;
    public int maxEnergyOnScene = 5;
    public float spawnInterval = 3f;

    [Header("Spawn Control")]
    private float timer;
    private int currentEnergyCount = 0;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && currentEnergyCount < maxEnergyOnScene)
        {
            SpawnEnergy();
            timer = spawnInterval;
        }
    }

    void SpawnEnergy()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(spawnAreaMin.position.x, spawnAreaMax.position.x),
            spawnAreaMin.position.y,
            Random.Range(spawnAreaMin.position.z, spawnAreaMax.position.z)
        );
        spawnPos.y += 0.5f;
        GameObject energy = Instantiate(energyPrefab, spawnPos, Quaternion.identity);
        currentEnergyCount++;
        EnergyPickup pickup = energy.GetComponent<EnergyPickup>();
        Debug.Log("Spawned energy. Current energy count: " + currentEnergyCount);
    
    if (pickup != null)
    {
        //pickup.spManager = PlaneControllerUnified.instance;
        //pickup.energySpawner = this; // optional, if not set in Start
    }
    }

     public void EnergyCollected()
     {
    currentEnergyCount = Mathf.Max(0, currentEnergyCount - 1);
    Debug.Log("Energy collected. Current energy count: " + currentEnergyCount);
     }   // When energy is collected or destroyed, reduce the count
       
    }

    // Called by EnergyPickupTracker when one is collected or destroyed
    