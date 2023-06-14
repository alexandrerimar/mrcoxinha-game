using System;
using System.IO;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{

    private GameManager gameManager;
    private MouseLook mouseLook;
    private FPSInput fpsInput;

    void Awake ()
    {
        gameManager = this.GetComponent<GameManager>();
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
                    gameManager.initialTimeWait = tempo;
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
                    gameManager.deltaTSuperior = deltaTSuperior;
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
                    gameManager.deltaTInferior = deltaTInferior;
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
                    gameManager.deltaTInicial = deltaTInicial;
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
                    gameManager.timeForChoice = tempoParaEscolher;
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
                    gameManager.TTotalDaTentativa = tempoTotalDaTentativa;
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
                    gameManager.totalSessions = sessoesTotais;
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
                    gameManager.totalBlocks = blocosTotais;
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
                    gameManager.totalAttempts = tentativasTotais;
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
                    gameManager.delayedDamage = danoMaximoStr;
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
                    gameManager.DanoSessao1 = danoSessaoStr;
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
                    gameManager.DanoSessao2 = danoSessao2Str;
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
                    gameManager.DanoSessao3 = danoSessao3Str;
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
                    gameManager.DanoSessao4 = danoSessao4Str;
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
                    gameManager.DanoSessao5 = danoSessao5Str;
                }
                else
                {
                    Debug.LogError("Invalid DanoSessao5 value: " + danoSessao5);
                }
                break;

            default:
                Debug.LogWarning("Unsupported configuration setting: " + key);
                break;
            
            
        }
    }
}
