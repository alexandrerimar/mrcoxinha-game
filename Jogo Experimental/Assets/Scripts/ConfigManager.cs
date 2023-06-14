using System.IO;
using UnityEngine;

public class ConfigManager
{
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
            Debug.LogWarning("Configuration file not found: " + fileName);
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
        // Apply the configuration setting in your game
        // Example: Setting the screen resolution
        if (key == "Resolution")
        {
            string[] resolutionValues = value.Split('x');
            if (resolutionValues.Length == 2)
            {
                int width = int.Parse(resolutionValues[0]);
                int height = int.Parse(resolutionValues[1]);
                Screen.SetResolution(width, height, Screen.fullScreen);
            }
        }
        // Add more logic to handle other settings as needed
    }
}
