using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeElapsedTime
{
    private DateTime tempoInicial; // Declare the DateTime variable at the class level

    public float ElapsedTime(bool inicio = true)
    {
        TimeSpan elapsedTime;
        float elapsedTimeFloat;
        DateTime tempoFinal;

        if (inicio)
        {
            tempoInicial = DateTime.Now;
            return 0f;
        }
        else
        {
            tempoFinal = DateTime.Now;
            elapsedTime = tempoFinal - tempoInicial;
            //Debug.Log("Elapsed Time: " + elapsedTime);
            elapsedTimeFloat = Convert.ToSingle(elapsedTime.TotalSeconds);
            //Debug.Log("Elapsed Time Converted: " + elapsedTimeFloat);

            return elapsedTimeFloat;
        }
    }
}