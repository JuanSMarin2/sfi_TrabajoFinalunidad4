using UnityEngine;
using System.IO.Ports;
using TMPro;

public class Serial : MonoBehaviour
{
    private SerialPort _serialPort = new SerialPort();
    private byte[] buffer = new byte[32];

    public TextMeshProUGUI myText;

    private static int counter = 0;

    void Start()
    {
        _serialPort.PortName = "COM4";
        _serialPort.BaudRate = 115200;
        _serialPort.DtrEnable = true;
        _serialPort.Open();
        Debug.Log("Open Serial Port");
    }

    void Update()
    {
        myText.text = counter.ToString();
        counter++;

        if (Input.GetKeyDown(KeyCode.A))
        {
            byte[] data = { 0x31 };// or byte[] data = {'1'};
            _serialPort.Write(data, 0, 1);
        }
        if (_serialPort.BytesToRead > 0)
        {
            int numData = _serialPort.Read(buffer, 0, 20);
            Debug.Log(System.Text.Encoding.ASCII.GetString(buffer));
            Debug.Log("Bytes received: " + numData.ToString());
        }
    }
}
