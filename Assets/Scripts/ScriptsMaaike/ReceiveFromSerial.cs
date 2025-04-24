using System.Collections;
using UnityEngine;
using System.Threading; // we need this library to make a thread to be able to get the data and not overload the memory in the the update()



public class ReceiveFromSerial : MonoBehaviour
{
   
   
    public delegate void OnSerialDataReceived(int value);
    public static event OnSerialDataReceived SerialDataReceived;
   // public static ReceiveFromSerial instance;  //Make into a Singleton




    private SerialPortManager spManager; // Get access to the serialport defined in the SerialPortManager    

    // Having data sent and recieved in a seperate thread to the main game thread stops unity from freezing
    Thread myThread; // we make the variable myThread with the type of Thread

    public float received = 0f; // we are going to change the speed with a potentialmeter

    // Start is called before the first frame update
    
    // void Awake()
    // {
    //     // Singleton pattern, so there is only one instance
    //     if (instance == null)
    //     {
    //         instance = this;
    //         DontDestroyOnLoad(gameObject); // Persist between scenes
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }
    
    void Start()
    {
        spManager = SerialPortManager.instance; // Obtain the serial port from the manager
        StartCoroutine(WaitForSerialPort());  // Start the Coroutine that is waiting for the serial port to be opend, before wecontinue with the script 
        myThread = new Thread(DataThread);

    }


    IEnumerator WaitForSerialPort()
    {
        while (!spManager.isReady)  // Waiting for the gate to be ready
        {
            Debug.Log("Waiting for serial port...");
            yield return new WaitForSeconds(0.5f);  // Wait 0.5 sec and check again
        }
        Debug.Log("Serial port is ready to receive!");

        myThread.Start();
    }
    void DataThread()
    {

        while (spManager.serialPort != null && spManager.serialPort.IsOpen)
        {
            spManager.serialPort.ReadTimeout = 2000;

            try
            {
                string value = spManager.serialPort.ReadLine(); // Read serial data
                int intValue = int.Parse(value); // Convert string to integer
                received = intValue;
                Debug.Log("received: " + received);
                SerialDataReceived?.Invoke(intValue);


            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log("exception!!!");
            }
        }
        Debug.Log("einde data");
    }


}
