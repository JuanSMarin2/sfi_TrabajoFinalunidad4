using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class ButtonMannager : MonoBehaviour
{

enum TaskState
{
    INIT,
    WAIT_COMMANDS
}

    bool ledState;

        private static TaskState taskState = TaskState.INIT;
private SerialPort _serialPort;
private byte[] buffer;

private int counter = 0;

void Start()
{

        int counter = 0;
    _serialPort = new SerialPort();
    _serialPort.PortName = "COM4";
    _serialPort.BaudRate = 115200;
    _serialPort.DtrEnable = true;
    _serialPort.NewLine = "\n";
    _serialPort.Open();
    Debug.Log("Open Serial Port");
    buffer = new byte[128];

        _serialPort.Write("outOFF\n");
    }

void Update()
{

        counter++;
   
    
}
    public void read()
    {
        _serialPort.Write("read\n");
        Debug.Log("Send read");


        if ( ledState == true)
        {
            Debug.Log(counter + " HIGH");
        }
        else if(ledState == false)
        {
            Debug.Log(counter + " LOW");
        }
    }

    public void On()
    {
        ledState = true;
        _serialPort.Write("outON\n");
        Debug.Log("Send outON ");

    }

   public void Of()
    {
        ledState = false;
        _serialPort.Write("outOFF\n");
        Debug.Log("Send outOFF ");

    }


}

