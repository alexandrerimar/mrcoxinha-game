using System;
using System.IO;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{

    //GameManager gameManager;
    MouseLook mouseLook;
    FPSInput fpsInput;

    [SerializeField] GameObject controller;
    GameManager gameManagerScript;

    void Start ()
    {
        //gameManager = GetComponent<GameManager>();
        gameManagerScript = controller.GetComponent<GameManager>();
        mouseLook = FindObjectOfType<MouseLook>();
        fpsInput = FindObjectOfType<FPSInput>();
    }


    public void LoadConfigurations(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Parse and process each line of the configuration file
                    ProcessConfigurationLine(line);
                }

                // Close the reader after reading the file
                reader.Close();
            }
        }
        else
        {
            Debug.LogError("Configuration file not found: " + fileName);
        }
    }

    private void ProcessConfigurationLine(string line)
    {
        // Split the line into key-value pairs based on the separator
        string[] keyValue = line.Split('=');
        if (keyValue.Length == 2)
        {
            string key = keyValue[0].Trim();
            string value = keyValue[1].Trim();

            // Apply the configuration setting in your game
            ApplyConfigurationSetting(key, value);
        }
    }

    private void ApplyConfigurationSetting(string key, string value)
    {
        switch (key)
        {
            case "Resolution":
                string[] resolutionValues = value.Split('x');
                int width, height;
                if (resolutionValues.Length == 2 &&
                    int.TryParse(resolutionValues[0], out width) &&
                    int.TryParse(resolutionValues[1], out height))
                {
                    if (width == 1920 && height == 1080)
                    {
                        Screen.SetResolution(width, height, Screen.fullScreen);
                    }
                    else
                    {
                        Debug.LogError("Invalid resolution value: " + value);
                    }
                }
                else
                {
                    Debug.LogError("Invalid resolution value: " + value);
                }
                break;

            case "TextureQuality":
                string textureQualityValue = value;
                if (string.Equals(textureQualityValue, "High", StringComparison.OrdinalIgnoreCase))
                {
                    QualitySettings.masterTextureLimit = 0;
                }
                else if (string.Equals(textureQualityValue, "Medium", StringComparison.OrdinalIgnoreCase))
                {
                    QualitySettings.masterTextureLimit = 1;
                }
                else if (string.Equals(textureQualityValue, "Low", StringComparison.OrdinalIgnoreCase))
                {
                    QualitySettings.masterTextureLimit = 3;
                }
                else
                {
                    Debug.LogError("Invalid texture quality value: " + value);
                }
                break;

            case "TempoAntesDeIniciarJogo":
                float tempo;
                if (float.TryParse(value, out tempo))
                {
                    gameManagerScript.initialTimeWait = tempo;
                }
                else
                {
                    Debug.LogError("Invalid TempoAntesDeIniciarJogo value: " + tempo);
                }
                break;

            case "DeltaTSuperior":
                float deltaTSuperior;
                if (float.TryParse(value, out deltaTSuperior))
                {
                    gameManagerScript.deltaTSuperior = deltaTSuperior;
                }
                else
                {
                    Debug.LogError("Invalid DeltaTSuperior value: " + deltaTSuperior);
                }
                break;
            
            case "DeltaTInferior":
                float deltaTInferior;
                if (float.TryParse(value, out deltaTInferior))
                {
                    gameManagerScript.deltaTInferior = deltaTInferior;
                }
                else
                {
                    Debug.LogError("Invalid DeltaTInferior value: " + deltaTInferior);
                }
                break;

            case "DeltaTInicial":
                float deltaTInicial;
                if (float.TryParse(value, out deltaTInicial))
                {
                    gameManagerScript.deltaTInicial = deltaTInicial;
                }
                else
                {
                    Debug.LogError("Invalid DeltaTInicial value: " + deltaTInicial);
                }
                break;
            
            case "TempoParaEscolher":
                float tempoParaEscolher;
                if (float.TryParse(value, out tempoParaEscolher))
                {
                    gameManagerScript.timeForChoice = tempoParaEscolher;
                }
                else
                {
                    Debug.LogError("Invalid TempoParaEscolher value: " + tempoParaEscolher);
                }
                break;
            
            case "TempoTotalDaTentativa":
                float tempoTotalDaTentativa;
                if (float.TryParse(value, out tempoTotalDaTentativa))
                {
                    gameManagerScript.TTotalDaTentativa = tempoTotalDaTentativa;
                }
                else
                {
                    Debug.LogError("Invalid TempoTotalDaTentativa value: " + tempoTotalDaTentativa);
                }
                break;

            case "SessoesTotais":
                int sessoesTotais;
                if (int.TryParse(value, out sessoesTotais))
                {
                    gameManagerScript.totalSessions = sessoesTotais;
                }
                else
                {
                    Debug.LogError("Invalid SessoesTotais value: " + sessoesTotais);
                }
                break;

            case "BlocosTotais":
                int blocosTotais;
                if (int.TryParse(value, out blocosTotais))
                {
                    gameManagerScript.totalBlocks = blocosTotais;
                }
                else
                {
                    Debug.LogError("Invalid BlocosTotais value: " + blocosTotais);
                }
                break;
            
            case "TentativasTotais":
                int tentativasTotais;
                if (int.TryParse(value, out tentativasTotais))
                {
                    gameManagerScript.totalAttempts = tentativasTotais;
                }
                else
                {
                    Debug.LogError("Invalid TentativasTotais value: " + tentativasTotais);
                }
                break;
            
            case "DanoMaximo":
                int danoMaximo;
                string danoMaximoStr;
                if (int.TryParse(value, out danoMaximo))
                {   
                    danoMaximoStr = danoMaximo.ToString();
                    gameManagerScript.delayedDamage = danoMaximoStr;
                }
                else
                {
                    Debug.LogError("Invalid DanoMaximo value: " + danoMaximo);
                }
                break;
            
            case "DanoSessao1":
                int danoSessao;
                string danoSessaoStr;
                if (int.TryParse(value, out danoSessao))
                {   
                    danoSessaoStr = danoSessao.ToString();
                    gameManagerScript.DanoSessao1 = danoSessaoStr;
                }
                else
                {
                    Debug.LogError("Invalid DanoSessao1 value: " + danoSessao);
                }
                break;
            
            case "DanoSessao2":
                int danoSessao2;
                string danoSessao2Str;
                if (int.TryParse(value, out danoSessao2))
                {   
                    danoSessao2Str = danoSessao2.ToString();
                    gameManagerScript.DanoSessao2 = danoSessao2Str;
                }
                else
                {
                    Debug.LogError("Invalid DanoSessao2 value: " + danoSessao2);
                }
                break;
            case "DanoSessao3":
                int danoSessao3;
                string danoSessao3Str;
                if (int.TryParse(value, out danoSessao3))
                {   
                    danoSessao3Str = danoSessao3.ToString();
                    gameManagerScript.DanoSessao3 = danoSessao3Str;
                }
                else
                {
                    Debug.LogError("Invalid DanoSessao3 value: " + danoSessao3);
                }
                break;
            
            case "DanoSessao4":
                int danoSessao4;
                string danoSessao4Str;
                if (int.TryParse(value, out danoSessao4))
                {   
                    danoSessao4Str = danoSessao4.ToString();
                    gameManagerScript.DanoSessao4 = danoSessao4Str;
                }
                else
                {
                    Debug.LogError("Invalid DanoSessao4 value: " + danoSessao4);
                }
                break;

            case "DanoSessao5":
                int danoSessao5;
                string danoSessao5Str;
                if (int.TryParse(value, out danoSessao5))
                {   
                    danoSessao5Str = danoSessao5.ToString();
                    gameManagerScript.DanoSessao5 = danoSessao5Str;
                }
                else
                {
                    Debug.LogError("Invalid DanoSessao5 value: " + danoSessao5);
                }
                break;
            
            case "DistanciaMaxima":
                float distanciaMaxima;
                if (float.TryParse(value, out distanciaMaxima))
                {   
                    gameManagerScript.globalMaxDistance = distanciaMaxima;
                }
                else
                {
                    Debug.LogError("Invalid DistanciaMaxima value: " + distanciaMaxima);
                }
                break;

            case "DistanciaMinima":
                float distanciaMinima;
                if (float.TryParse(value, out distanciaMinima))
                {   
                    gameManagerScript.globalMinDistance = distanciaMinima;
                }
                else
                {
                    Debug.LogError("Invalid DistanciaMinima value: " + distanciaMinima);
                }
                break;
            
            case "MouseSensibilidadeHor":
                float mouseSensibilidadeHor;
                if (float.TryParse(value, out mouseSensibilidadeHor))
                {   
                    mouseLook.sensitivityHor = mouseSensibilidadeHor;
                }
                else
                {
                    Debug.LogError("Invalid MouseSensibilidadeHor value: " + mouseSensibilidadeHor);
                }
                break;
            
            case "MouseSensibilidadeVert":
                float mouseSensibilidadeVer;
                if (float.TryParse(value, out mouseSensibilidadeVer))
                {   
                    mouseLook.sensitivityVert = mouseSensibilidadeVer;
                }
                else
                {
                    Debug.LogError("Invalid MouseSensibilidadeVert value: " + mouseSensibilidadeVer);
                }
                break;
            
            case "VelocidadeDeCaminhada":
                float velocidadeDeCaminhada;
                if (float.TryParse(value, out velocidadeDeCaminhada))
                {   
                    fpsInput.speed = velocidadeDeCaminhada;
                }
                else
                {
                    Debug.LogError("Invalid VelocidadeDeCaminhada value: " + velocidadeDeCaminhada);
                }
                break;

            default:
                Debug.LogWarning("Unsupported configuration setting: " + key);
                break;
            
            
        }
    }
}
