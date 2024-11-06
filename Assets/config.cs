using UnityEngine;
using System.IO.Ports;
using TMPro;
using System;
using System.Threading;
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
    public TextMeshProUGUI frameCounterText; // TextMeshProUGUI para mostrar el frameCounter

    private float counter = 300.0f;
    private int numTemp = 1;
    private float numInun = 1.0f;
    private int frameCounter = 0; // Contador de frames

    bool inicio;
    private bool ackReceived = false; // Para rastrear si se ha recibido el ACK de Arduino
    private Thread frameCounterThread; // Hilo para el frameCounter
    private bool isRunning = true; // Flag para detener el hilo

    void Start()
    {
        _serialPort = new SerialPort();
        _serialPort.PortName = "COM5";
        _serialPort.BaudRate = 115200;
        _serialPort.DtrEnable = true;
        _serialPort.NewLine = "\n";
        _serialPort.Open();
        Debug.Log("Open Serial Port");
        buffer = new byte[128];

        // Iniciar el hilo del frameCounter
        frameCounterThread = new Thread(IncrementFrameCounter);
        frameCounterThread.Start();
    }

    void Update()
    {
        if (numInun == -4) numInun = 1;
        if (numTemp == -4) numTemp = 1;
        if (numInun >= 4) numInun = 1;
        if (numTemp >= 4) numTemp = 1;

        myText.text = counter != -5 ? counter.ToString() : "Desactivado";
        Inunda.text = numInun != -5 ? numInun.ToString() : "Desactivado";
        Temper.text = numTemp != -5 ? "Activado" : "Desactivado";

        // Actualizar frameCounterText en la interfaz
        frameCounterText.text = "Frame Counter: " + frameCounter.ToString();

        switch (taskState)
        {
            case TaskState.INIT:
                taskState = TaskState.WAIT_COMMANDS;
                _serialPort.Write("reset\n");
                Debug.Log("reset");
                Debug.Log("WAIT COMMANDS");
                break;

            case TaskState.WAIT_COMMANDS:
                if (Input.GetKeyDown(KeyCode.S) && counter < 500)
                {
                    counter++;
                }
                if (Input.GetKeyDown(KeyCode.B) && counter > 100)
                {
                    counter--;
                }

                // Cuando se presiona la tecla L, envía los datos al microcontrolador
                if (inicio)
                {
                    SendData(counter, numInun, numTemp);
                    SceneManager.LoadScene("Preguntas");
                }
                break;

            default:
                Debug.Log("State Error");
                break;
        }
    }

    // Método que se ejecuta en el hilo para incrementar el frameCounter
    void IncrementFrameCounter()
    {
        while (isRunning)
        {
            frameCounter++;
            if (frameCounter >= 100000)
            {
                frameCounter = 0;
            }
            Thread.Sleep(16); // Similar a un incremento por frame en 60 FPS
        }
    }

    // Método para enviar los datos en formato little-endian
    void SendData(float counter, float numInun, float numTemp)
    {
        float checksum = counter + numInun + numTemp;
        byte[] dataToSend = new byte[16];
        byte[] counterBytes = BitConverter.GetBytes(counter);
        byte[] numInunBytes = BitConverter.GetBytes(numInun);
        byte[] numTempBytes = BitConverter.GetBytes(numTemp);
        byte[] checksumBytes = BitConverter.GetBytes(checksum);

        Buffer.BlockCopy(counterBytes, 0, dataToSend, 0, 4);
        Buffer.BlockCopy(numInunBytes, 0, dataToSend, 4, 4);
        Buffer.BlockCopy(numTempBytes, 0, dataToSend, 8, 4);
        Buffer.BlockCopy(checksumBytes, 0, dataToSend, 12, 4);

        _serialPort.Write(dataToSend, 0, dataToSend.Length);
        Debug.Log("Data sent: " + BitConverter.ToString(dataToSend));
    }

    public void aumentar() { if (counter != -5 && counter < 350) counter += 0.5f; else counter = 350.0f; }
    public void disminuir() { if (counter != -5 && counter > 100) counter--; else counter = 100.0f; }
    public void maximo() { counter = 350.0f; }
    public void iniciar() { inicio = true; }
    public void minimo() { counter = 100; }
    public void temperatura() { numTemp++; }
    public void inundacion() { numInun = numInun + 0.5f; }
    public void desactCounter() { counter = -5; }
    public void desactTemp() { numTemp = -5; }
    public void desactInun() { numInun = -5; }

    void OnDestroy()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            Debug.Log("Serial port closed in OnDestroy.");
        }

        isRunning = false; // Detener el hilo
        frameCounterThread.Join(); // Esperar a que el hilo termine
    }

    void OnApplicationQuit()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            Debug.Log("Serial port closed in OnApplicationQuit.");
        }

        isRunning = false; // Detener el hilo
        frameCounterThread.Join(); // Esperar a que el hilo termine
    }
}
