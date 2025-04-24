using System.Collections;
using UnityEngine;


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




    void Start()
    {
        lastTimeFired = Time.time;
        startRotation = transform.rotation;

    }


    void Update()
    {
        if (KeyBoardUse)
        {
            HandleKeyboardInput();
        }
        else HandleSerialInput();

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
            //  SendData("L");
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            newPosition.x += moveSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(rotationAmount);
            // SendData("R");
        }

        transform.position = newPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);

        if (Input.GetKey(KeyCode.Space))
        {
            Fire();
        }
    }


    void HandleSerialInput()
    {
        Quaternion targetRotation = Quaternion.identity;

        if (value == 1) // Left
        {
            targetRotation = Quaternion.Euler(-rotationAmount);
            StartCoroutine(SmoothMove(Vector3.left, 0.2f)); // beweeg soepel
            StartCoroutine(ResetRotationAfterDelay());
            value = 0;
        }
        else if (value == 2) // Right
        {
            targetRotation = Quaternion.Euler(rotationAmount * 10);
            StartCoroutine(SmoothMove(Vector3.right, 0.2f));
            StartCoroutine(ResetRotationAfterDelay());
            value = 0;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);

        if (value == 3) // Shoot
        {
            Fire();
            value = 0;
        }
    }

    IEnumerator SmoothMove(Vector3 direction, float duration)
    {
        float elapsed = 0f;
        float localSpeed = moveSpeed * 0.1f; // adjust this percentage for less extreme movement

        while (elapsed < duration)
        {
            transform.position += direction * (localSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ResetRotationAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // small delay
        float t = 0f;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * 2; // turning back slower 
            transform.rotation = Quaternion.Lerp(startRot, startRotation, t);
            yield return null;
        }

        transform.rotation = startRotation;
    }


    // }
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
