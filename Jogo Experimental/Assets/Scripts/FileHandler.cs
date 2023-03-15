using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public static class FileHandler {
    
    public static void SaveToJSON<T> (List<T> toSave, string filename) {
        // Salva a informação no arquivo JSON especificado

        Debug.Log (GetPath (filename));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(filename), content);
    }

    public static void SaveToJSON<T> (T toSave, string filename) {
        // Salva a informação no arquivo JSON especificado

        Debug.Log (GetPath (filename));
        string content = JsonUtility.ToJson (toSave);
        WriteFile(GetPath(filename), content);
    }


    public static List<T> ReadListFromJSON<T> (string filename) {
        string content = ReadFile(GetPath(filename));
        
        if (string.IsNullOrEmpty(content) || content == "{}") {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        
        return res;

    }

    public static T ReadFromJSON<T> (string filename) {
        string content = ReadFile(GetPath(filename));
        
        if (string.IsNullOrEmpty(content) || content == "{}") {
            return default(T);
        }

        T res = JsonUtility.FromJson<T>(content);
        
        return res;

    }

    private static string GetPath (string filename) {
        // Retorna o caminho para salvar o arquivo JSON

        return Application.persistentDataPath + "/" + filename;
    }

    private static void WriteFile (string path, string content) {
        // Cria o arquivo JSON se ele não existe e escreve a informação passada
        
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter (fileStream)) {
            writer.Write(content);
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
