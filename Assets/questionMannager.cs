using UnityEngine;
using System.IO.Ports;
using TMPro;
using System.Threading;
using UnityEngine.SceneManagement;

public class questionMannager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip soundClip1;
    public AudioClip soundClip2;

    private SerialPort _serialPort;
    public TextMeshProUGUI counterText; // Para mostrar el contador
    public TextMeshProUGUI tempText;    // Para mostrar la temperatura real
    public TextMeshProUGUI inunText;    // Para mostrar la inundación
    public TextMeshProUGUI preguntaText;
    public GameObject inunda2;
    public GameObject inunda1;

    public GameObject[] QObjects = new GameObject[15];

    private float counter = 0;
    private float numInun = 0;
    private float realTemp = 0.0f;
    private float actualInun = -1;
    private int contador = 0;

    private bool isRunning = true;
    private bool isInunReceived = false; // Nueva bandera para controlar la recepción de inundación
    private Thread dataThread;

    void Start()
    {
        // Configuración del puerto serie
        _serialPort = new SerialPort
        {
            PortName = "COM5",
            BaudRate = 115200,
            DtrEnable = true,
            NewLine = "\n"
        };
        _serialPort.Open();
        Debug.Log("Open Serial Port");

        // Iniciar el hilo para la recepción de datos
        dataThread = new Thread(ReceiveData);
        dataThread.Start();
    }

    void Update()
    {
        // Actualizar UI en el hilo principal
        preguntaText.text = contador.ToString() + "/15";
        counterText.text = counter == -5 ? "Contador: Desactivado" : "Contador: " + counter.ToString();
        tempText.text = realTemp == -5 ? "Temperatura: Desactivado" : "Temperatura: " + realTemp.ToString("F2") + "°";

        inunText.text = "Inundación: " + actualInun.ToString() + "%";
        switch (actualInun)
        {
            case 1:
                inunda1.SetActive(true);
                inunda2.SetActive(false);
                break;
            case 2:
                inunda2.SetActive(true);
                inunda1.SetActive(false);
                break;
            case 3:
                inunda1.SetActive(false);
                inunda2.SetActive(false);
                break;
            case 0:
                SceneManager.LoadScene("GameOver");
                break;
        }

        if (contador >= 1 && contador <= 15)
        {
            QObjects[contador - 1].SetActive(false);
        }

        if (contador >= 15)
        {
            SceneManager.LoadScene("Victoria");
        }
    }

    void ReceiveData()
    {
        while (isRunning)
        {
            if (_serialPort.BytesToRead > 0)
            {
                string response = _serialPort.ReadLine(); // Leer la línea enviada desde Arduino
                if (!string.IsNullOrEmpty(response))
                {
                    string[] values = response.Split(',');

                    // Procesar los datos si se recibieron los tres valores esperados
                    if (values.Length == 3)
                    {
                        if (float.TryParse(values[0], out float receivedCounter))
                        {
                            counter = receivedCounter / 100.0f;
                        }

                        if (!isInunReceived && float.TryParse(values[1], out float receivedInun))
                        {
                            // Solo asignar el valor de inundación la primera vez que se recibe
                            actualInun = receivedInun > 0 ? receivedInun / 100.0f : actualInun;
                            numInun = receivedInun;
                            isInunReceived = true; // Marcar que ya se recibió la primera vez
                        }

                        if (float.TryParse(values[2], out float receivedTemp))
                        {
                            realTemp = receivedTemp / 100.0f;
                        }
                    }
                }
            }
            Thread.Sleep(10); // Pequeña pausa para evitar consumo excesivo de CPU
        }
    }

    public void OnMistakeButtonClick()
    {
        if (actualInun > 0)
        {
            actualInun -= 0.5f;
        }

        audioSource.clip = soundClip1;
        audioSource.Play();

        Debug.Log("Mistake " + actualInun);
    }

    public void Correcto()
    {
        Debug.Log("Good " + contador);
        contador++;

        audioSource.clip = soundClip2;
        audioSource.Play();
    }

    void OnDestroy()
    {
        isRunning = false;
        if (dataThread != null && dataThread.IsAlive)
        {
            dataThread.Join();
        }

        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            Debug.Log("Serial port closed in OnDestroy.");
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            Debug.Log("Serial port closed in OnApplicationQuit.");
        }
    }
}
