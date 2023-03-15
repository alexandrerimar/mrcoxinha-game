using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attempt : MonoBehaviour
{



    public bool isChosen; //verifica se uma escolha foi feita
    public bool activeChoice = false; //permitido escolher?
    public float choiceTime = 5f; // tempo para escolher
    public int imediateDamage = 10; // muda a cada fase
    public int delayedDamage = 100; // constante
    public int waitTime = 7; // alterado de acordo com escolhas em blocos
    public int attemptNumber = 0; // 0 -> outras, 1 -> escolha 1, 2 -> escolha 2

    public enum ChoiceSelector
    {
          //Fazer o enum funcionar, caso não dê, use uma lista ou tuple msm. Mais simples.
          Waiting, //0
          Nenhuma, //1
          Imediata, //2
          Atrasada, //3
          
    }
    public ChoiceSelector selecaoAtual;


    // Event para informar escolha
    public delegate void ResultadoFinalizado ();
    public static event ResultadoFinalizado OnResultadoFinalizado;
    public bool resultadoFinalizado = false;

    //Outros

   
    void Update()          
     {
          
          if (resultadoFinalizado == true)
          {
               if (OnResultadoFinalizado != null)
               {
                    OnResultadoFinalizado();
               }
          }
          

          if (activeChoice == true) {
               if (attemptNumber == 0) {
                    //selecaoAtual = ChoiceSelector.Waiting;
                    if (Input.GetMouseButtonDown(0)) {
                         // Escolha imediata
                         selecaoAtual = ChoiceSelector.Imediata;
                         isChosen = true; // informa que foi escolhido                    
                         activeChoice = false; // desativa a permissão pra escolher
                         resultadoFinalizado = true;
                         // Debug.Log("Opção imediata foi escolhida. Causou " + imediateDamage + " de dano.");
                    
                    } 
                    else if (Input.GetMouseButtonDown(1)) {
                         // escolha atrasada
                         selecaoAtual = ChoiceSelector.Atrasada;
                         isChosen = true;
                         activeChoice = false; // desativa a permissão pra escolher
                         resultadoFinalizado = true;
                         // Debug.Log("Opção atrasada foi escolhida. Causou " + delayedDamage + " de dano.");
                    }
               }
               else if (attemptNumber == 1) {
                    if (Input.GetMouseButtonDown(0)) {
                         // Escolha imediata forçada
                         selecaoAtual = ChoiceSelector.Imediata;
                         isChosen = true; // informa que foi escolhido                    
                         activeChoice = false; // desativa a permissão pra escolher
                         resultadoFinalizado = true;
                         // Debug.Log("Opção imediata foi escolhida. Causou " + imediateDamage + " de dano.");
                    }
               }
               else if (attemptNumber == 2) {
                    if (Input.GetMouseButtonDown(1)) {
                         // escolha atrasada forçada
                         selecaoAtual = ChoiceSelector.Atrasada;
                         isChosen = true;
                         activeChoice = false; // desativa a permissão pra escolher
                         resultadoFinalizado = true;
                         // Debug.Log("Opção atrasada foi escolhida. Causou " + delayedDamage + " de dano.");
                    }
               }
               
          }  
     }    

    public IEnumerator TimeForChoiceCoroutine()
    {
          // Espera o tempo determinado por choiceTime
          yield return new WaitForSeconds(choiceTime);

          //Escolha não foi feita
          activeChoice = false; // desativa a permissão pra escolher
          selecaoAtual = ChoiceSelector.Nenhuma;
          resultadoFinalizado = true;
          // Debug.Log("Tempo limite excedido, escolha não foi feita.");

          
    }
}
