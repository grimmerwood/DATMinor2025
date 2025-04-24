using System.Collections;
using UnityEngine;
using System.Threading; // we need this library to make a thread to be able to get the data and not overload the memory in the the update()



public class ReadFromSerial : MonoBehaviour
{
    private SerialPortManager spManager; // Get access to the serialport defined in the SerialPortManager    

    // Having data sent and recieved in a seperate thread to the main game thread stops unity from freezing
    Thread myThread; // we make the variable myThread with the type of Thread


    // Start is called before the first frame update
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
               
                Debug.Log("Received from seria: " + value );
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
