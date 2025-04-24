using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingSerialInput : MonoBehaviour
{

    public int value; // This value we can use for what ever we want


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
