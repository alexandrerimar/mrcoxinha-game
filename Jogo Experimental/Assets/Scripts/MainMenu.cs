using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public string playerID;
   [SerializeField] Button playButton;
   public static string playerIDCrossScene;

   void Start() {
      playButton.interactable = false;
   }
   
   private void ReadStringInput (string s) {
      // Passa o valor do campo de imput para a variável playerID
      playerID = s;
      Debug.Log(playerID);
   }
   
   private void activatePlayButton () {
      // Ativa interação do botão Play quando há pelo menos 9 caracteres no input
      if (!string.IsNullOrEmpty(playerID)) {
         if (playerID.Length >= 9) {
            playButton.interactable = true;
         }
      }
   }

   private void PlayGame () {
      playerIDCrossScene = playerID;
      SceneManager.LoadScene("EscolhaImpulsivaDD"); //Posso fazer aleatório.
   }

   private void QuitGame() {
      Debug.Log("Quit!");
      Application.Quit();
   }
}
