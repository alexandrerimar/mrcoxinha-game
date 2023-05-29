using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   [SerializeField] GameObject sceneLoader;
   SceneLoader sceneLoaderScript;   
   [SerializeField] Button playButton;
   
   public string playerID;   
   public static string playerIDCrossScene;

   void Start() {
      playButton.interactable = false;
      sceneLoaderScript = sceneLoader.GetComponent<SceneLoader>();
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
      sceneLoaderScript.LoadNextScene();
   }

   private void QuitGame() {
      Debug.Log("Quit!");
      Application.Quit();
   }
}
