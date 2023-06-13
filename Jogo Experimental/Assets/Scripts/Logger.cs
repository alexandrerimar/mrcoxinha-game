using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger
{
    private static Logger instance;
    private static readonly object padlock = new object();
    private StreamWriter defaultLogFile;
    private Dictionary<string, StreamWriter> logFiles;
    private string defaultLogFileName;

    private Logger()
    {
        logFiles = new Dictionary<string, StreamWriter>();
    }

    public static Logger Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }
    }

    public void SetDefaultLogFileName(string logFileName)
    {
        defaultLogFileName = logFileName;
    }

    public void LogAction(string action)
    {
        if (defaultLogFile == null)
        {
            if (string.IsNullOrEmpty(defaultLogFileName))
            {
                defaultLogFileName = "DefaultLogFile.log";
            }

            // Create the default log file when the first action log is made
            string logFilePath = Path.Combine(Application.persistentDataPath, defaultLogFileName);
            defaultLogFile = File.AppendText(logFilePath);
        }

        Debug.Log(DateTime.Now + ": " + action);
        defaultLogFile.WriteLine(DateTime.Now + ": " + action);
        defaultLogFile.Flush();
    }

    public void LogAction(string logFileName, string action)
    {
        if (string.IsNullOrEmpty(logFileName))
        {
            LogAction(action); // Use the default log file if no log file name is provided
            return;
        }

        StreamWriter writer;
        if (!logFiles.TryGetValue(logFileName, out writer))
        {
            // Create a new log file if it doesn't exist
            string logFilePath = Path.Combine(Application.persistentDataPath, logFileName);
            writer = File.AppendText(logFilePath);
            logFiles.Add(logFileName, writer);
        }

        Debug.Log(DateTime.Now + ": " + action);
        writer.WriteLine(DateTime.Now + ": " + action);
        writer.Flush();
    }

    public void CloseLogFiles()
    {
        if (defaultLogFile != null)
        {
            defaultLogFile.Close();
            defaultLogFile.Dispose();
            defaultLogFile = null;
        }

        foreach (StreamWriter writer in logFiles.Values)
        {
            writer.Close();
            writer.Dispose();
        }
        logFiles.Clear();
    }
}
