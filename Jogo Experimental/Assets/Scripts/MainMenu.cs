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

   int activeSceneIndex;

   void Start() {
      playButton.interactable = false;
      sceneLoaderScript = sceneLoader.GetComponent<SceneLoader>();
      activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
   }
   
   public void ReadStringInput (string s) {
      // Passa o valor do campo de imput para a variável playerID
      playerID = s;
      Debug.Log(playerID);
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
      playerIDCrossScene = playerID;
      sceneLoaderScript.LoadSceneByPositionOnList(activeSceneIndex);
   }

   private void QuitGame() {
      Debug.Log("Quit!");
      Application.Quit();
   }
}
