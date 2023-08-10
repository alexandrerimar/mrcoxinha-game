using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public static class FileHandler {
    
    public static void SaveToJSON<T> (List<T> toSave, string filename, string folderName) {
        // Salva a informação no arquivo JSON especificado

        Debug.Log (GetPath (filename, folderName));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(filename, folderName), content);
    }

    public static void SaveToJSON<T> (T toSave, string filename, string folderName) {
        // Salva a informação no arquivo JSON especificado

        Debug.Log (GetPath (filename, folderName));
        string content = JsonUtility.ToJson (toSave);
        WriteFile(GetPath(filename, folderName), content);
    }


    public static List<T> ReadListFromJSON<T> (string filename, string folderName) {
        string content = ReadFile(GetPath(filename, folderName));
        
        if (string.IsNullOrEmpty(content) || content == "{}") {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        
        return res;

    }

    public static T ReadFromJSON<T> (string filename, string folderName) {
        string content = ReadFile(GetPath(filename, folderName));
        
        if (string.IsNullOrEmpty(content) || content == "{}") {
            return default(T);
        }

        T res = JsonUtility.FromJson<T>(content);
        
        return res;

    }
/*
    private static string GetUniqueFolderPath(string baseFolderPath, string folderName)
    {
        string folderPath = Path.Combine(baseFolderPath, folderName);

        int counter = 1;
        while (Directory.Exists(folderPath))
        {
            folderPath = Path.Combine(baseFolderPath, $"{folderName}({counter})");
            counter++;
        }

        return folderPath;
    }

    private static string GetPath(string filename, string folderName)
    {
        string baseFolderPath = Application.persistentDataPath;
        string uniqueFolderPath = GetUniqueFolderPath(baseFolderPath, folderName);

        return Path.Combine(uniqueFolderPath, filename);
    }
*/

    
    private static string GetPath (string filename, string folderName = "default") {
        // Retorna o caminho para salvar o arquivo JSON

        return Application.persistentDataPath + "/" + folderName + "/" + filename;
    }

    /*
    private static void WriteFile (string path, string content) {
        // Cria o arquivo JSON se ele não existe e escreve a informação passada
        
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter (fileStream)) {
            writer.Write(content);
        }
    }
    */
    private static void WriteFile(string path, string content)
    {
        try
        {
            string directoryPath = Path.GetDirectoryName(path);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                // POSSÍVEL SOLUÇÃO: CRIAR O DIRETÓRIO NO MENU, E LÁ VERIFICAR SE ELE JÁ EXISTE. AQUI, NÃO CRIAR NENHUM DIRETÓRIO, SÓ ARQUIVOS.
            }

            // Open the file stream and write the content
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write(content);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error writing file: " + ex.Message);
        }
    }


    private static string ReadFile (string path) {
        if (File.Exists(path)) {
            using (StreamReader reader = new StreamReader (path)) {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
   


    public static class JsonHelper {
        // Remove o Array the objetos do top-level
        // Permite salvar uma lista de objetos ao invés de somente um objeto

        public static T[] FromJson<T>(string json) {
            // Pega os objetos de dentro do wrapper
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array) {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint) {
            // Converte um Array genérico em um item da classe genérica Wrapper
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T> {
            // A classe Wrapper é um Array genérico.
            // Ela pega um Array de objetos e joga dentro de outro array
            // Assim, os dados não ficam no top-level do arquivo Json
            // E é possível colocar vários objetos dentro desse array criado.
            public T[] Items;
        }
        }
    
}
