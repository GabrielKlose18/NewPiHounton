using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO.Ports;
public class SerialController : MonoBehaviour
{
    SerialPort arduinoPort = new SerialPort("/dev/cu.usbserial-1410"); // ou 1420
    
    private void Awake(){
        arduinoPort.BaudRate = 9600;
        arduinoPort.Parity = Parity.None;
        arduinoPort.StopBits = StopBits.None;
        arduinoPort.DataBits = 8;
        arduinoPort.Handshake = Handshake.None;
    }
    // Start is called before the first frame update
    void Start(){
        arduinoPort.Open();
        // arduinoPort.ReadTimeout = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendMessageToArduino(string msg){
        arduinoPort.WriteLine(msg);
    }

    public void ClosePort(){
        arduinoPort.Close();
    }
}
