using System;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public static Logger instance;
    public string filepath = "Logger/";
    public string participant = "Name";
    public string condition = "1";

    //* private stuff *//

    private StreamWriter sw;

    public static void LogMessage(string message)
    {
        instance.log(message);
    }

    public static void LogLine(string message)
    {
        instance.logLine(message);
    }

    private void Awake()
    {
        instance = this;
        string filename = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
                            + "_" + condition + "_" + participant
                            + ".txt";
        print(filename);
        sw = new StreamWriter(filepath + filename);
    }

    private void OnDestroy()
    {
        sw.Close();
        sw.Dispose();
    }

    public void log(string message)
    {
        print("Logging  Message: " + message);
        sw.Write(message);
    }

    public void logLine(string message)
    {
        message = System.DateTime.Now.ToString("HH:mm:ss") + "|" + message;
        print("Logging Line: " + message);
        sw.WriteLine(message);
    }
}