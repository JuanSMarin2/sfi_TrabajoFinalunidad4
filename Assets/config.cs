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

    bool inicio;

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
    }

    void Update()
    {



        if(numInun==-4){
            numInun=1;
        }
            if(numTemp==-4){
            numTemp=1;
        }
     if(numInun==-4){
            numInun=1;
        }
            if(numTemp==-4){
            numTemp=1;
        }
     if(numInun>=4){
            numInun=1;
        }
            if(numTemp>=4){
            numTemp=1;
        }


        


        if(counter != -5){ 
        myText.text = counter.ToString();

        }else{
            myText.text = "Desactivado".ToString();
        }

        if(numInun != -5){
        Inunda.text = numInun.ToString();
        }else{
            Inunda.text = "Desactivado".ToString(); 
        }
        if(numTemp != -5){
        Temper.text = numTemp.ToString();
        }else{
             Temper.text = "Desactivado".ToString();
        }

        switch (taskState)
        {
            case TaskState.INIT:
                taskState = TaskState.WAIT_COMMANDS;
                 _serialPort.Write("reset\n");
                    Debug.Log("reset");
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

                // Cuando se presiona la tecla L, env�a los datos al microcontrolador
                if (inicio == true)
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

    public void aumentar(){
        if (counter!=-5 && counter < 350){

        counter++;

        } else {
            
            counter = 350;
        }
    }

    public void disminuir(){
       if (counter!=-5 && counter > 100){

        counter--;

        } else {
            counter = 100;
        }
    }

    public void maximo()
    {
        counter = 350;
    }

     public void iniciar()
    {
        inicio= true;
    }

    public void minimo()
    {
        counter = 100;
    }

    public void temperatura()
    {
       
        
            numTemp++;
        

    }

    public void inundacion()
    {
     
        
            numInun++;
        

    }

    public void desactCounter(){

        counter =-5;
    }

      public void desactTemp(){

        numTemp =-5;
    }

      public void desactInun(){

        numInun =-5;
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
