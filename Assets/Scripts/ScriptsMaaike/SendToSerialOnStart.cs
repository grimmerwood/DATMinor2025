using System.Collections;
using UnityEngine;


public class SendToSerialOnStart : MonoBehaviour
{
    private SerialPortManager spManager; // Get access to the serialport defined in the SerialPortManager    

     [Header("Message to send to the serialPort on Start")]
    public string messageToSerial; // What message to send to the serialPort

    // Start is called before the first frame update
    void Start()
    {   
        spManager = SerialPortManager.instance; // Obtain the serial port from the manager
        StartCoroutine(WaitForSerialPort());  // Start the Coroutine that is waiting for the serial port to be opend, before wecontinue with the script 
        SendData(messageToSerial); //Sends the message to the serial
    }

    IEnumerator WaitForSerialPort()
    {
        while (!spManager.isReady)  // Waiting for the gate to be ready
        {
            Debug.Log("Waiting for serial port...");
            yield return new WaitForSeconds(0.5f);  // Wait 0.5 sec and check again
        }
        Debug.Log("Serial port is ready!");
    }

    public void SendData(string message)
    {
        StartCoroutine(WaitAndSend(message));
    }

    IEnumerator WaitAndSend(string message)
    {
        while (!spManager.isReady)  // Extra check to avoid sending too early
        {
            Debug.Log("Waiting before sending data...");
            yield return null;  // Wait a frame and try again
        }
        if (spManager.serialPort != null && spManager.serialPort.IsOpen && !string.IsNullOrWhiteSpace(message))
        {
            try
            {
                spManager.serialPort.WriteLine(message);
                spManager.serialPort.BaseStream.Flush();
                Debug.Log("Send: " + message);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error writing to serial port: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open! or there is no message to send");
        }
    }




}
