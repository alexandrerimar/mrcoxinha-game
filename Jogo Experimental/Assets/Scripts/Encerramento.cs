using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Encerramento : MonoBehaviour
{
   [SerializeField] GameObject sceneLoader;
   SceneLoader sceneLoaderScript;   
   
   int activeSceneIndex;

   public void QuitGameEncerramento() {
      Logger.Instance.LogAction("Quit!");
      Logger.Instance.CloseLogFiles();
      Application.Quit();
   }
}
