using System;
using System.IO;
using UnityEngine;

public class Logger
{
    private static Logger instance;
    private static readonly object padlock = new object();
    private StreamWriter writer;

    private Logger()
    {
        string logFile = "PlayerActions.log";

        // Verifica se o arquivo de log já existe
        if (File.Exists(logFile))
        {
            // Se já existir, adiciona as informações ao final do arquivo
            writer = File.AppendText(logFile);
        }
        else
        {
            // Se não existir, cria um novo arquivo
            writer = File.CreateText(logFile);
        }
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

    public void LogAction(string action)
    {
        Debug.Log(DateTime.Now + ": " + action);
        writer.WriteLine(DateTime.Now + ": " + action);
        writer.Flush();
    }
}