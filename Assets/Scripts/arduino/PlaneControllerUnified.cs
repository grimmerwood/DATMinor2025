using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneControllerUnified : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float moveSpeed = 3f;
    public Vector3 rotationAmount = new Vector3(0, 45f, 0);
    public float moveDistance = 1.5f;
    public float alertDistance = 5f;

    public string port = "/dev/tty.usbserial-2110";
    public int bautrate = 9600;

    private SerialPort serialPort;
    private PlaneControllerUnified playerController;
    private float lastTimeFired;
    private GameObject[] enemies;      // Cache enemy list
    private float enemyRefreshRate = 2f;  // Refresh every 2 seconds
    private float lastEnemyRefreshTime;
    private bool alerted = false;
    private Quaternion startRotation;

    [SerializeField]
    private bool KeyBoardUse = false;
    public bool isPlayerAlive = true;
    
    public static PlaneControllerUnified instance;

    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        lastTimeFired = Time.time;
        
        startRotation = transform.rotation;
        StartCoroutine(OpenSerialPort());

        RefreshEnemies(); // Initial cache
        lastEnemyRefreshTime = Time.time;
    }
    void RefreshEnemies()
    {
    enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
    IEnumerator OpenSerialPort()
    {
        serialPort = new SerialPort(port, bautrate)
        {
            ReadTimeout = 100,
            WriteTimeout = 100
        };

        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened.");
        }
        catch
        {
            Debug.LogError("Failed to open serial port.");
        }

        yield return new WaitForSeconds(1f);
    }

    void Update()
    {
        if (!isPlayerAlive)
    {
        // Accept restart from either keyboard or serial input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Restart key detected");
            UIController.Instance.RestartLevel();
        }
        return; // Prevent further input when dead
    }

    if (KeyBoardUse)
    {
        HandleKeyboardInput();
    }
    else
    {
        HandleSerialInput();
    }

    DetectEnemyProximity();
    }

    void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Missing projectile prefab.");
            return;
        }

        ProjectileSettings settings = projectilePrefab.GetComponent<ProjectileSettings>();
        if (settings == null)
        {
            Debug.LogWarning("ProjectileSettings missing.");
            return;
        }

        if (Time.time - lastTimeFired > settings.firingSpeed)
        {
            lastTimeFired = Time.time;
            Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            SendSerialCommand("H");  // Notify Arduino: bullet hit enemy
        }
    }

    void HandleKeyboardInput()
    {
        if (!isPlayerAlive) // If the player is dead
        {
            if (Input.GetKey(KeyCode.Space)) // Space bar to restart when dead
            {
              UIController.Instance.RestartLevel();
            } 
        }
        else // If the player is alive
        {
           Vector3 newPosition = transform.position;
           Quaternion targetRotation = Quaternion.identity;

           if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
           {
              newPosition.x -= moveSpeed * Time.deltaTime;
              targetRotation = Quaternion.Euler(-rotationAmount);
              SendSerialCommand("L");
           }

           if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
           {
              newPosition.x += moveSpeed * Time.deltaTime;
              targetRotation = Quaternion.Euler(rotationAmount);
              SendSerialCommand("R");
           }

           transform.position = newPosition;
           if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || 
              Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
           {
              transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
           }
           else
          {
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * 10);
          }

          if (Input.GetKey(KeyCode.Space))
          {
            Fire();
          }
        }
   }

    void HandleSerialInput()
    {
        if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
          try
          {
            string data = serialPort.ReadLine().Trim();
            Debug.Log("Arduino says: " + data);

            if (data == "L")
                transform.position += Vector3.left * moveDistance;

            else if (data == "R")
                transform.position += Vector3.right * moveDistance;

            else if (data == "S")  // 💥 Arduino shoot signal
            {
                if (isPlayerAlive)
                {
                    Fire();
                }
                else
                {
                    UIController.Instance.RestartLevel(); // If player is dead, restart the game
                }
            }
          } 
            catch { }
        }
    }
    
    void DetectEnemyProximity()
    {
        // Refresh enemy list every few seconds
       if (Time.time - lastEnemyRefreshTime > enemyRefreshRate)
       {
         RefreshEnemies();
         lastEnemyRefreshTime = Time.time;
       }
        if (enemies == null || enemies.Length == 0) return;
        
        bool anyEnemyClose = false;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < alertDistance)
            {
                anyEnemyClose = true;
                if (!alerted)
                {
                    //Debug.Log("E");
                    SendSerialCommand("E");
                    alerted = true;
                }
                break;
            }
        }

        if (!anyEnemyClose)
        //Debug.Log("E");
            alerted = false;
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Energy"))
        {
            Debug.Log("C");
            SendSerialCommand("C");
        }
    }
    

    public void SendSerialCommand(string command)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(command);
            Debug.Log("Sent to Arduino: " + command);
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }
}
