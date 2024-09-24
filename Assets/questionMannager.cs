using UnityEngine;
using System.IO.Ports;
using TMPro;
using UnityEngine.SceneManagement;

public class questionMannager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip soundClip1;
    public AudioClip soundClip2;

    private SerialPort _serialPort;
    public TextMeshProUGUI counterText; // Para mostrar el contador
    public TextMeshProUGUI tempText;    // Para mostrar la temperatura
    public TextMeshProUGUI inunText;    // Para mostrar la inundaci�n

    public TextMeshProUGUI preguntaText;   
    public GameObject inunda2;
    public GameObject inunda1;
    public GameObject temp2;
    public GameObject temp1;

 public GameObject[] QObjects = new GameObject[15];

    private int counter = 0;
    private int numTemp = 0;
    private int numInun = 0;

    private int actualTemp = 0;
    private int actualInun = 0;

    private int displayInun = 0;
    private int displayTemp = 0;

    int contador =0;

    bool variablesTransfered = false; //Hace que las variables solo se guarden una vez y no se sobreescriban en cada frame

    void Start()
    {
        // Configuraci�n del puerto serie
        _serialPort = new SerialPort();
        _serialPort.PortName = "COM5"; 
        _serialPort.BaudRate = 115200;
        _serialPort.DtrEnable = true;
        _serialPort.NewLine = "\n";
        _serialPort.Open();
        Debug.Log("Open Serial Port");
    }

    void Update()
    {

         preguntaText.text = contador.ToString()+"/15";

          if (contador >= 1 && contador <= 15)
        {
            // Desactiva el objeto correspondiente (Q1 = índice 0, Q15 = índice 14)
            QObjects[contador - 1].SetActive(false);
        }
        if(contador >= 15 ){
               SceneManager.LoadScene("Victoria");
        }

    
        // Enviar el comando "update" al Arduino cuando se presione la tecla S
        if (Input.GetKeyDown(KeyCode.S))
        {
            _serialPort.Write("update\n"); // Enviar el comando "update" a Arduino
            Debug.Log("Send update");
        }

        // Leer datos desde el microcontrolador
        if (_serialPort.BytesToRead > 0)
        {
            string response = _serialPort.ReadLine(); // Leer la l�nea enviada desde Arduino
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
                         if(variablesTransfered == false){
                        numInun = receivedInun;
                        actualInun = numInun;
                         }
                    }

                    if (int.TryParse(values[2].Split(':')[1], out int receivedTemp))
                    {
                        if(variablesTransfered == false){
                        numTemp = receivedTemp;
                        actualTemp = numTemp;



                        variablesTransfered = true;
                        }
                    }

                    // Actualizar los valores en la UI
                 // Manejo de 'actualTemp'
switch(actualTemp)
{
    case 1:
        temp1.SetActive(true);
        displayTemp = 100;
        break;
    case 2:
        temp2.SetActive(true);
        displayTemp = 60;
        break;
    case 3:
        displayTemp = 26;
        break;
}

// Manejo de 'actualInun'
switch(actualInun)
{
    case 1:
        inunda1.SetActive(true);
        inunda2.SetActive(false);
        displayInun = 90;
        break;
    case 2:
        inunda2.SetActive(true);
        displayInun = 40;
        break;
    case 3:
        displayInun = 0;
        break;
    case 0:
        SceneManager.LoadScene("GameOver");
        break;
}


                if(counter == 0){
                SceneManager.LoadScene("GameOver");
                }



// Para el contador
if (counter == -5)
{
    counterText.text = "Contador: Desactivado";
}
else
{
    counterText.text = "Contador: " + counter.ToString();
}

// Para la inundación
if (actualInun == -5)
{
    inunText.text = "Inundación: Desactivado";
}
else
{
    inunText.text = "Inundación: " + displayInun.ToString() + "%";
}

// Para la temperatura
if (actualTemp == -5)
{
    tempText.text = "Temperatura: Desactivado";
}
else
{
    tempText.text = "Temperatura: " + displayTemp.ToString() + "°";
}


                }
            }
        }
    }


 public void OnMistakeButtonClick()
    {

        
        if(actualTemp > 0)
        {
            actualTemp--;
        }
        if(actualInun > 0)
        {
            actualInun--;
        }
                
        audioSource.clip = soundClip1;
        audioSource.Play();

 Debug.Log("Mistake " + actualTemp
 + actualInun);
    }

   public void Correcto(){
         Debug.Log("Good " + contador);
        contador++;

        audioSource.clip = soundClip2;
        audioSource.Play();
    }



    void OnDestroy() {
    if (_serialPort != null && _serialPort.IsOpen) {
        _serialPort.Close();
        Debug.Log("Serial port closed in OnDestroy.");
    }
}

void OnApplicationQuit() {
    if (_serialPort != null && _serialPort.IsOpen) {
        _serialPort.Close();
        Debug.Log("Serial port closed in OnApplicationQuit.");
    }
}


}
