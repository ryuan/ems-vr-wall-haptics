using System.IO.Ports;
using UnityEngine;

/*
 * A simple script to send messages over serial port
 */

public class SerialInterface : MonoBehaviour
{
    public bool useSerial = false;
    public int timeout = 500;
    private SerialPort serialport;
    private bool ready = true;
    private float timer = 0;
    public int broadrate = 9600;
    public string testmessage = "1";
    public KeyCode testKey;
    public string serialPortName = "COM4";

    private static SerialInterface instance;

    public static new void SendMessage(string message)
    {
        instance.sendSerialMessage(message);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    private void Start()
    {
        instance = this;
        if (useSerial)
        {
            serialport = new SerialPort(serialPortName, broadrate);
            serialport.WriteTimeout = 1000;
            serialport.Open();
        }
    }

    // Send a message over serial port
    public void sendSerialMessage(string message)
    {
        if (!useSerial || timer > 0)
            return;

        timer = timeout;
        print("sending message through serial port:" + message);
        serialport.WriteLine(message);
    }

    private void OnDestroy()
    {
        if (useSerial)
        {
            print("closing serial port");
            serialport.Close();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            sendSerialMessage(testmessage);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (serialport != null)
                serialport.Close();
            Start();
            return;
        }
        timer -= Time.deltaTime * 1000;
        timer = Mathf.Max(0, timer);
    }
}