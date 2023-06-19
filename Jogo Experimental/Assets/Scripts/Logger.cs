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

    public void LogAction(string action, string logFileName = null)
    {
        StreamWriter writer;

        if (string.IsNullOrEmpty(logFileName))
        {
            if (defaultLogFile == null)
            {
                // Create the default log file when the first action log is made
                string logName = defaultLogFileName + ".log";
                string logFilePath = Path.Combine(Application.persistentDataPath, logName);
                defaultLogFile = File.AppendText(logFilePath);
                //logFiles.Add(defaultLogFileName + ".log", defaultLogFile);
            }

            Debug.Log(DateTime.Now + ": " + action);
            defaultLogFile.WriteLine(DateTime.Now + ": " + action);
            defaultLogFile.Flush();
        }
        else if (logFiles.TryGetValue(logFileName, out writer))
        {
            Debug.Log(DateTime.Now + ": " + action);
            writer.WriteLine(DateTime.Now + ": " + action);
            writer.Flush();
        }
        else
        {
            /// Use the persistent data path as the log directory            
            string logFilePath = Path.Combine(Application.persistentDataPath, logFileName);
            
            writer = File.AppendText(logFilePath);
            logFiles.Add(logFileName, writer);

            Debug.Log(DateTime.Now + ": " + action);
            writer.WriteLine(DateTime.Now + ": " + action);
            writer.Flush();
        }
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
