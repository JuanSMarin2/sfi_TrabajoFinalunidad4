using UnityEngine;
using System.IO.Ports;
using TMPro;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

enum TaskState
{
    INIT,
    WAIT_COMMANDS
}

public class config : MonoBehaviour
{
    private static TaskState taskState = TaskState.INIT;
    private SerialPort _serialPort;
    private byte[] buffer;
    public TextMeshProUGUI myText;
    public TextMeshProUGUI Temper;
    public TextMeshProUGUI Inunda;

    private int counter = 300;
    private int numTemp = 1;
    private int numInun = 1;

    void Start()
    {
        _serialPort = new SerialPort();
        _serialPort.PortName = "COM8";
        _serialPort.BaudRate = 115200;
        _serialPort.DtrEnable = true;
        _serialPort.NewLine = "\n";
        _serialPort.Open();
        Debug.Log("Open Serial Port");
        buffer = new byte[128];
    }

    void Update()
    {
        myText.text = counter.ToString();
        Inunda.text = numInun.ToString();
        Temper.text = numTemp.ToString();

        switch (taskState)
        {
            case TaskState.INIT:
                taskState = TaskState.WAIT_COMMANDS;
                Debug.Log("WAIT COMMANDS");
                break;

            case TaskState.WAIT_COMMANDS:
                if (Input.GetKeyDown(KeyCode.S) && counter < 350)
                {
                    counter++;
                }
                if (Input.GetKeyDown(KeyCode.B) && counter > 100)
                {
                    counter--;
                }

                // Cuando se presiona la tecla L, envía los datos al microcontrolador
                if (Input.GetKeyDown(KeyCode.L))
                {
                    string dataToSend = $"L,{counter},{numInun},{numTemp}\n";
                    _serialPort.Write(dataToSend);
                    Debug.Log("Sent data: " + dataToSend);

                    SceneManager.LoadScene("Preguntas");

                }

                if (_serialPort.BytesToRead > 0)
                {
                    string response = _serialPort.ReadLine();
                    Debug.Log(response);
                }
                break;

            default:
                Debug.Log("State Error");
                break;
        }
    }

    public void maximo()
    {
        counter = 350;
    }

    public void minimo()
    {
        counter = 100;
    }

    public void temperatura()
    {
        if (numTemp < 3)
        {
            numTemp++;
        }
        else
        {
            numTemp = 1;
        }
    }

    public void inundacion()
    {
        if (numInun < 3)
        {
            numInun++;
        }
        else
        {
            numInun = 1;
        }
    }
}
