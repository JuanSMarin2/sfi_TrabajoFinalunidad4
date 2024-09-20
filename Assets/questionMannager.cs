using UnityEngine;
using System.IO.Ports;
using TMPro;

public class questionMannager : MonoBehaviour
{
    private SerialPort _serialPort;
    public TextMeshProUGUI counterText; // Para mostrar el contador
    public TextMeshProUGUI tempText;    // Para mostrar la temperatura
    public TextMeshProUGUI inunText;    // Para mostrar la inundación
    private int counter = 0;
    private int numTemp = 0;
    private int numInun = 0;

    private int actualTemp = 0;
    private int actualInun = 0;

    private int displayInun = 0;
    private int displayTemp = 0;

    void Start()
    {
        // Configuración del puerto serie
        _serialPort = new SerialPort();
        _serialPort.PortName = "COM4"; 
        _serialPort.BaudRate = 115200;
        _serialPort.DtrEnable = true;
        _serialPort.NewLine = "\n";
        _serialPort.Open();
        Debug.Log("Open Serial Port");
    }

    void Update()
    {
        // Enviar el comando "update" al Arduino cuando se presione la tecla S
        if (Input.GetKeyDown(KeyCode.S))
        {
            _serialPort.Write("update\n"); // Enviar el comando "update" a Arduino
            Debug.Log("Send update");
        }

        // Leer datos desde el microcontrolador
        if (_serialPort.BytesToRead > 0)
        {
            string response = _serialPort.ReadLine(); // Leer la línea enviada desde Arduino
            Debug.Log("Received from Arduino: " + response);

            // Si la respuesta contiene los valores de counter, numInun, y numTemp
            if (response.StartsWith("counter:"))
            {
                // Dividir la respuesta por comas
                string[] values = response.Substring(9).Split(',');

                // Asegurarse de que se recibieron los 3 valores
                if (values.Length == 3)
                {
                    // Extraer los valores de counter, numInun, y numTemp
                    if (int.TryParse(values[0], out int receivedCounter))
                    {
                        counter = receivedCounter;
                    }

                    if (int.TryParse(values[1].Split(':')[1], out int receivedInun))
                    {
                        numInun = receivedInun;
                        actualInun = numInun;
                    }

                    if (int.TryParse(values[2].Split(':')[1], out int receivedTemp))
                    {
                        numTemp = receivedTemp;
                        actualTemp = numTemp;
                    }

                    // Actualizar los valores en la UI




                    if (actualInun == numInun) {

                        displayInun = 0;

                    }

                    if (actualTemp == numTemp)
                    {

                        displayInun = 26;

                    }

                    counterText.text = "Contador: " + counter.ToString();
                    inunText.text = "Inundacion: " + displayInun.ToString() + "°";
                    tempText.text = "Temperatura: " + displayTemp.ToString();
                }
            }
        }
    }


    void mistake()
    {
        if(actualTemp > 0)
        {
            actualTemp--;

        } else if(actualTemp<0 && actualInun > 0)
        {
            actualInun--;

        }
        else if (actualTemp==0 && actualInun==0) {
        //lose

        }

    }

}
