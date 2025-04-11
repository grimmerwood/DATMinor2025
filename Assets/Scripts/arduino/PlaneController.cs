using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public static PlaneController instance;
    //public SerialPort serial = new SerialPort("/dev/tty.usbserial-2110", 9600); // Change this to your actual port
    public float moveSpeed = 3f; // Speed of movement
    public float rotationAngle = 15f; // Rotation effect
    public float moveDistance = 1.5f; // Distance to move per press

    private Vector3 startPosition;
    private Quaternion startRotation;
    public float alertDistance = 5f;
    private bool alerted = false;

    public SerialPort serialPort;
    public bool isReady = false;

    public string port = "/dev/tty.usbserial-2110"; // making a public variable with a type tekst allows us to change the port in Unity
    public int bautrate = 9600; // making a public variable with a type a whole number, allows us to change the bautrate in Unity


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
        startPosition = transform.position;
        startRotation = transform.rotation;
        Debug.Log("start has happened");
        StartCoroutine(OpenSerialPort());


    }
    IEnumerator OpenSerialPort()
    {
        serialPort = new SerialPort(port, bautrate)
        {
            ReadTimeout = 100, // Optional: prevents ReadLine() from sticking
            WriteTimeout = 100 // Optional: prevents Write() from sticking
        };
        serialPort.Open(); //this opens the serial port
        Debug.Log("port was opened successfully");


        // Please wait before sending data
        System.Threading.Thread.Sleep(1000);

        yield return new WaitForSeconds(1f);  // Wait 1 second to be sure
        isReady = true;

    }

    void Update()
    {

        HandleSerialInput();   // Move plane from Arduino button input
        HandleKeyboardInput(); // Move & shoot from keyboard
        DetectEnemyProximity(); // Send sound warning to Arduino
    }
    void HandleSerialInput()
    {
        if (serialPort.IsOpen && serialPort.BytesToRead > 0) // Only read if data is available
        {
            try
            {
                string data = serialPort.ReadLine().Trim(); // Read Arduino data and clean it
                Debug.Log("Received: " + data);

                if (data == "L")
                {
                    MoveLeft(); // Move left
                }
                else if (data == "R")
                {
                    MoveRight(); // Move right
                }
            }
            catch (System.Exception) { } // Prevent errors from stopping Unity
        }
    }
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.D)) MoveRight();

    }
    void MoveLeft()
    {
        transform.position += Vector3.left * moveDistance; // Move left
        transform.rotation = Quaternion.Euler(0, -rotationAngle, 0); // Tilt left
        StartCoroutine(ResetRotation());
    }

    void MoveRight()
    {
        transform.position += Vector3.right * moveDistance; // Move right
        transform.rotation = Quaternion.Euler(0, rotationAngle, 0); // Tilt right
        StartCoroutine(ResetRotation());
    }

    IEnumerator ResetRotation()
    {
        yield return new WaitForSeconds(0.5f); // Small delay
        transform.rotation = startRotation; // Reset rotation back to normal
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            PlayCrashSound();
        }

    }

    void DetectEnemyProximity()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool anyEnemyClose = false;

        foreach (var enemy in enemies)
        {

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < alertDistance)
            {
                anyEnemyClose = true;

                if (!alerted)
                {
                    SendSerialCommand("E"); // Send warning signal once
                    alerted = true;
                }

                break;
            }
        }

        if (!anyEnemyClose)
        {
            alerted = false; // Reset alert if no enemy is close anymore
        }
    }
    public void PlayCrashSound()
    {
        if (serialPort.IsOpen)
            serialPort.Write("C");
    }


    public void SendSerialCommand(string command)
    {
        if (serialPort.IsOpen)
        {
            Debug.Log("Sending command: " + command);
            serialPort.WriteLine(command);
        }
    }

    void OnApplicationQuit()
    {
        serialPort.Close(); // Close Serial when Unity stops
        Debug.Log("Portclosed");
    }

    public void PlayEnemyHitSound()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write("H"); // 'E' tells Arduino to play enemy scream
            Debug.Log("Shooting");
        }
    }
}