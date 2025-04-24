using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialPlayerController : MonoBehaviour
{

    [Header("This we don't fill in, but we can see here what values we receive")]
    public int value; // This value we can use for what ever we want
    private float amountToMove;
    [Header("change the speed of the movement the player makes")]  
    public int speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        amountToMove = speed * Time.deltaTime;
       MoveObject(value);
    }
    void MoveObject(int value)
    {
        if (value ==1)
        {
            transform.Translate(Vector3.left * amountToMove, Space.World);
            this.value = 0; // Reset na beweging
        }
        if (value ==2)
        {
            transform.Translate(Vector3.right * amountToMove, Space.World);         
            this.value = 0; // Reset na beweging   
        }
        if (value ==3)
        
        {
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
