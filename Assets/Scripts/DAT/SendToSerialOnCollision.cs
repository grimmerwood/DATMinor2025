using System.Collections;
using UnityEngine;


public class SendToSerialOnCollision : MonoBehaviour
{
    private SerialPortManager spManager; // Get access to the serialport defined in the SerialPortManager
    private string messageToSerial = "B"; // What message to send to the serialPort
    private float sendCooldown = 0.5f; // Time between transmissions in seconds
    private float lastSentTime; // Time of last transmission


    // Start is called before the first frame update
    void Start()
    {
        spManager = SerialPortManager.instance;  // Obtain the serial port from the manager                                              
    }

    public void SendData(string message)  //an own made methode that takes a parameter of a string which is defined as a variable called "message"
    {
        StartCoroutine(SendSerialMessage(message));
    }
    IEnumerator SendSerialMessage(string message) //Coroutine, a special methods that can temporarily pause and resume later without blocking the main thread of the game.
    {
        while (!spManager.isReady)  // Extra check to avoid sending too early
        {
            Debug.Log("Waiting before sending data...");
            yield return null;  // Wait a frame and try again
        }

        if (spManager.serialPort != null && spManager.serialPort.IsOpen)
        {
            try
            {
                spManager.serialPort.WriteLine(message);
                spManager.serialPort.BaseStream.Flush();
                Debug.Log("Send: " + message);
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

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastSentTime > sendCooldown ) // Check cooldown
        {
            Debug.Log("Collision detected with: " + collision.gameObject.name);
            SendData(messageToSerial);
        }
        else
        {
            Debug.Log("transmit blocked (cooldown active).");
        }

    }




}
