using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class PlaneControllerUnified : MonoBehaviour
{
    [Header("This we don't fill in, but we can see here what values we receive")]
    public int value; // This value we can use for what ever we want

    
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float moveSpeed = 3f;
    public Vector3 rotationAmount = new Vector3(0, 45f, 0);
    public float moveDistance = 1.5f;
    public float alertDistance = 5f;

    private float lastTimeFired;
    private bool alerted = false;
    private Quaternion startRotation;

    [SerializeField]
    private bool KeyBoardUse = false;

    public static PlaneControllerUnified instance;

    public bool isPlayerAlive = true;
    private float lastSentTime; // Time of last transmission

    private Health healthScript;

    private Vector3 moveTarget;
    private bool isMoving = false;
    private float lastInputTime; // To handle debouncing in Unity side
    private float lastSerialInputTime = 0f;

    public ObjectPool bulletPool; // Assign this in Inspector (pool with projectilePrefab)
    
    private List<GameObject> enemies = new List<GameObject>();
    void Start()
    {
        healthScript = GetComponent<Health>();
        lastTimeFired = Time.time;
        startRotation = transform.rotation;
        lastInputTime = Time.time; // Initialize last input time
        
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }


    void Update()
    {
        if (KeyBoardUse)
        {
            HandleKeyboardInput();
        }
        else HandleSerialInput();
        
        if (Time.frameCount % 10 == 0)
        {
            DetectEnemyProximity();
        }

        if (isMoving)
        {
           transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
           if (transform.position == moveTarget) isMoving = false;
        }
    }

    void Fire()
    {
        if (bulletPool == null)
        {
            Debug.LogWarning("Bullet pool not assigned.");
            return;
        }

        if (Time.time - lastTimeFired > projectilePrefab.GetComponent<ProjectileSettings>().firingSpeed)
        {
            lastTimeFired = Time.time;
            
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = spawnPoint.position;  
            bullet.transform.rotation = Quaternion.identity;
            
            SendToSerial("H"); // Notify Arduino: bullet hit enemy
            Debug.Log("H send to Arduino");
            
        }
    }

    void HandleKeyboardInput()
    {
        Vector3 newPosition = transform.position;
        Quaternion targetRotation = Quaternion.identity;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            newPosition.x -= moveSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(-rotationAmount);
            lastInputTime = Time.time; // Update the input time to prevent bounce
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            newPosition.x += moveSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(rotationAmount);
            lastInputTime = Time.time; // Update the input time to prevent bounce
        }

        transform.position = newPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);

        if (Input.GetKeyDown(KeyCode.Space) && healthScript != null && !healthScript.IsDead())
        {
            Fire();
        }
    }


    void HandleSerialInput()
    {
        //if (Time.time - lastSerialInputTime < serialInputDebounceTime) return; // Skip if debounce is active
       
       // Only allow movement or actions if enough time has passed since the last input
        if (Time.time - lastSerialInputTime < 0.1f) // 100ms debounce for movement
            return;
        Quaternion targetRotation = Quaternion.identity;

        if (value == 1) // Left
        {
           moveTarget = transform.position + Vector3.left * moveDistance;
           isMoving = true;
           targetRotation = Quaternion.Euler(-rotationAmount);
           value = 0;
           lastSerialInputTime = Time.time; // Update the debounce timer
        }
        else if (value == 2) // Right
        {
           moveTarget = transform.position + Vector3.right * moveDistance;
           isMoving = true;
           targetRotation = Quaternion.Euler(rotationAmount);
           value = 0;
           lastSerialInputTime = Time.time; // Update the debounce timer
       }

       if (value == 3) // Shoot
       {
          Fire();
          value = 0;
       }

       transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
       //lastSerialInputTime = Time.time;  // Update debounce timer for other actions (not shooting)
    }

    void StartMove(Vector3 direction)
    {
        Quaternion targetRotation = direction == Vector3.left ? Quaternion.Euler(-rotationAmount) : Quaternion.Euler(rotationAmount);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction * moveDistance, moveSpeed * Time.deltaTime);
        value = 0;
    }


    // }
    void DetectEnemyProximity()
    {
       
        bool anyEnemyClose = false;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < alertDistance)
            {
                anyEnemyClose = true;
                if (!alerted)
                {
                    Debug.Log("E is send to Arduino");
                    SendToSerial("E");

                    alerted = true;
                }
                break;
            }
        }

        if (!anyEnemyClose)
        {     
            alerted = false;
        }

    }


    
        public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Energy"))
        {
            Debug.Log("C is send to Arduino");
            SendToSerial("C");
        }
    }



    void SendToSerial(string message)
    {
        if (SerialPortManager.instance != null &&
            SerialPortManager.instance.serialPort != null &&
            SerialPortManager.instance.serialPort.IsOpen)
        {
            try
            {
                SerialPortManager.instance.serialPort.WriteLine(message);
                SerialPortManager.instance.serialPort.BaseStream.Flush(); // Zorgt voor directe verzending
                Debug.Log("Sent to serial: " + message);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to send to serial: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port not ready.");
        }
    }



    void OnEnable()
    {
        ReceiveFromSerial.SerialDataReceived += HandleSerialInput;
    }

    void OnDisable()
    {
        ReceiveFromSerial.SerialDataReceived -= HandleSerialInput;
    }

    void HandleSerialInput(int receivedValue)
    {
        value = receivedValue; // Put the incoming value in the field shown by the inspector
        Debug.Log("Received in other script: " + value);
    }




}
