using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class ArduinoController : MonoBehaviour
{
    SerialPort serial = new SerialPort("/dev/tty.usbmodem11101", 9600); // Change "COM3" to your Arduino port
    public GameObject airplane; // Drag your plane object here

    void Start()
    {
        serial.Open(); // Open Serial Communication
        serial.ReadTimeout = 100; // Prevent freezing
    }

    void Update()
    {
        if (serial.IsOpen)
        {
            try
            {
                string data = serial.ReadLine(); // Read Arduino data
                Debug.Log("Received: " + data);

                if (data == "L")  
                    airplane.transform.Rotate(0, -10, 0); // Turn left

                if (data == "R")  
                    airplane.transform.Rotate(0, 10, 0); // Turn right
            }
            catch (System.Exception) { } // Prevent Unity from freezing
        }
    }

    void OnApplicationQuit()
    {
        serial.Close(); // Close Serial when Unity stops
    }
}
