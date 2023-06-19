using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   [SerializeField] GameObject sceneLoader;
   SceneLoader sceneLoaderScript;   
   [SerializeField] Button playButton;
   
   public string playerID;   
   public static string playerIDCrossScene;
   public static string nomeDoLogDeMovimento;

   int activeSceneIndex;

   void Awake() {
      playButton.interactable = false;
      sceneLoaderScript = sceneLoader.GetComponent<SceneLoader>();
      activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
   }
   
   public void ReadStringInput (string s) {
      // Passa o valor do campo de imput para a variável playerID
      playerID = s;
   }
   
   public void activatePlayButton () {
      // Ativa interação do botão Play quando há pelo menos 9 caracteres no input
      if (!string.IsNullOrEmpty(playerID)) {
         if (playerID.Length >= 9) {
            playButton.interactable = true;
         }
      }
   }

   public void PlayGame () {
      Logger.Instance.LogAction("Cena Atual: " + activeSceneIndex);
      playerIDCrossScene = playerID;
      Logger.Instance.SetDefaultLogFileName(playerIDCrossScene);
      Logger.Instance.LogAction("PlayerID: " + playerIDCrossScene);

      nomeDoLogDeMovimento = playerIDCrossScene + "_movimento.log";
      Logger.Instance.LogAction("PlayerID: " + playerIDCrossScene, nomeDoLogDeMovimento);      
      
      sceneLoaderScript.LoadSceneByPositionOnList(activeSceneIndex);
   }

   private void QuitGame() {
      Logger.Instance.LogAction("Quit!");
      Logger.Instance.CloseLogFiles();

      Application.Quit();
   }
}
