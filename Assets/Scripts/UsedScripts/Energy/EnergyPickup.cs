using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyPickup : MonoBehaviour
{
    private SerialPortManager spManager; // Get access to the serialport defined in the SerialPortManager    
    private float lastSentTime; // Time of last transmission

   
    public int healAmount = 1;

    public GameObject healthPopupPrefab;
    
    private float startY;
    private float pickupDelay = 0.5f; // wait before being collectible
    private float spawnTime;  

    private bool hasBeenCollected = false;  
    
    void Start()
    
    {
        startY = transform.position.y;
    }




   
    void OnCollisionEnter(Collision other)
    {      
       if (hasBeenCollected) return;      
       if (Time.time - spawnTime < pickupDelay) return; // too soon, ignore    
        if (other.gameObject.CompareTag("Player") )
        {  
            // Mark as collected
            hasBeenCollected = true;
            
            Health playerHealth = other.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
               playerHealth.Heal(healAmount);
               Debug.Log("Player healed by: " + healAmount);
            }
            // Cache position BEFORE destroying this GameObject
            Vector3 cachedPosition = transform.position;
           
           // Get canvas and convert world position to screen position
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null && healthPopupPrefab != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                GameObject popup = Instantiate(healthPopupPrefab, screenPos, Quaternion.identity, canvas.transform);
            
            // ✅ Set the text to "+1 Health"
            TMP_Text textComponent = popup.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = "+" + healAmount + " Health";
            }
            
            }
            
        
        }
           
            
            
            PlaneControllerUnified plane = PlaneControllerUnified.instance;
            //if (plane != null)
            
                
            if (spManager != null)
            {
                SendData("C");
            }
                //plane.SendSerialCommand("C");
                  // Send 'C' to Arduino

            
            Destroy(gameObject);  // Energy disappears

    }

    public void SendData(string message)  //an own made methode that takes a parameter of a string which is defined as a variable called "message"
    {
        if (Time.time - lastSentTime >= 0.1f)  // 100 ms cooldown
        {
            StartCoroutine(SendSerialMessage(message));
        }
        
    }
    IEnumerator SendSerialMessage(string message) //Coroutine, a special methods that can temporarily pause and resume later without blocking the main thread of the game.
    {
        while (!spManager.isReady)  // Extra check to avoid sending too early
        {
            //Debug.Log("Waiting before sending data...");
            yield return null;  // Wait a frame and try again
        }

        if (spManager.serialPort != null && spManager.serialPort.IsOpen)
        {
            try
            {
                spManager.serialPort.WriteLine(message);
                //spManager.serialPort.BaseStream.Flush();
                //Debug.Log("Send: " + message);
                lastSentTime = Time.time; // Update the last time it was sent
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error writing to serial port: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open!");
        }
    }

     }

    

