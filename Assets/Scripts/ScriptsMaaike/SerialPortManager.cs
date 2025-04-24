using System.Collections;
using System.IO.Ports;
using UnityEngine;

public class SerialPortManager : MonoBehaviour
{
    public static SerialPortManager instance;
    public SerialPort serialPort;
    public bool isReady = false;

    public string port = "/dev/tty.usbserial-11240"; // making a public variable with a type tekst allows us to change the port in Unity
    public int bautrate = 9600; // making a public variable with a type a whole number, allows us to change the bautrate in Unity

    [Header("Message to send to the serialPort on Close")]
    public string messageToSerial; // What message to send to the serialPort
    
    void Awake()
    {
        // Singleton pattern, so there is only one instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(OpenSerialPort());
    }


    IEnumerator OpenSerialPort()
    {
        serialPort = new SerialPort(port, bautrate)
        {
            ReadTimeout = 1000, // Optional: prevents ReadLine() from sticking
            WriteTimeout = 100 // Optional: prevents Write() from sticking
        };
        serialPort.Open(); //this opens the serial port
        Debug.Log("port was opened successfully");


        // Please wait before sending data
        System.Threading.Thread.Sleep(1000);

        yield return new WaitForSeconds(1f);  // Wait 1 second to be sure
        isReady = true;       

    }



void OnApplicationQuit()
{
    if (serialPort != null && serialPort.IsOpen)
    {
        if (!string.IsNullOrWhiteSpace(messageToSerial))
        {
            try
            {
                serialPort.Write(messageToSerial);
                serialPort.BaseStream.Flush(); // Optional but clean
                Debug.Log("Message sent before closing: " + messageToSerial);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to send message on quit: " + e.Message);
            }
        }

        serialPort.Close();
        Debug.Log("Serial port closed!");
    }
}


}
