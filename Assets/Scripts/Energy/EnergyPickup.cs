using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    public PlaneControllerUnified spManager;
    public float floatSpeed = 5f;
    public float floatHeight = 0.5f;
    public float rotationSpeed = 45f;
    public int healAmount = 5;
    public float moveSpeedZ= -1f; // Speed at which it moves toward player
    public GameObject healthPopupPrefab;
    public EnergySpawner energySpawner;
    private float startY;
    private float pickupDelay = 0.2f; // wait before being collectible
    private float spawnTime;    
    
    void Start()
    {
        spManager = PlaneControllerUnified.instance;  // Obtain the serial port from the manager                                              
        // Align pickup height with player's Y on spawn
        spawnTime = Time.time;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
    {
        Vector3 pos = transform.position;
        pos.y = player.transform.position.y;  // Match Y
        transform.position = pos;

        startY = pos.y;
    }
    else
    {
        startY = transform.position.y;
    }

    
    }
    
    
    void Update()
    {

       // Floating up and down on Y
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        Vector3 pos = transform.position;
        pos.y = startY + yOffset;

        // Move forward in Z direction
        pos.z += moveSpeedZ * Time.deltaTime;

        transform.position = pos;

        // Rotate for effect
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }


   
    void OnCollisionEnter(Collision other)
    {            
       if (Time.time - spawnTime < pickupDelay)
       return; // too soon, ignore    
        if (other.gameObject.CompareTag("Player"))
        {  
            Health playerHealth = other.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
               playerHealth.Heal(healAmount);
               Debug.Log("Player healed by: " + healAmount);
            }
           // Get canvas and convert world position to screen position
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null && healthPopupPrefab != null)
            {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            GameObject popup = Instantiate(healthPopupPrefab, screenPos, Quaternion.identity, canvas.transform);
            }
            
        }
            
            
           EnergySpawner spawner = FindObjectOfType<EnergySpawner>();
           if (energySpawner != null)
        {
             energySpawner.EnergyCollected();
        }
            
            
            PlaneControllerUnified plane = PlaneControllerUnified.instance;
            //if (plane != null)
            
                
            if (spManager != null)
            {
                spManager.SendSerialCommand("C");
            }
                //plane.SendSerialCommand("C");
                  // Send 'C' to Arduino

            
            Destroy(gameObject);  // Energy disappears

        }
     }

    

