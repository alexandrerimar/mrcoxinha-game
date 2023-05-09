using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attempt : MonoBehaviour
{
    public bool isChosen; //verifica se uma escolha foi feita
    public bool activeChoice = false; //permitido escolher?
    public int attemptNumber = 0; // 0 -> outras, 1 -> escolha 1, 2 -> escolha 2
    //public int imediateDamage = 10; // muda a cada fase
    //public int delayedDamage = 100; // constante
    //public int waitTime = 7; // alterado de acordo com escolhas em blocos
    

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

    //Botões
     [SerializeField] Button agoraBtn;
     [SerializeField] Button depoisBtn;

     //Referência a Game Manager
     [SerializeField] GameObject enemy;
     private bool isEnemyActive;


     void Start () {

          agoraBtn.gameObject.SetActive(false);
          depoisBtn.gameObject.SetActive(false);
     }

    void Update()          
     {   
          isEnemyActive = enemy.activeSelf;

          if (resultadoFinalizado == true)
          {
               if (OnResultadoFinalizado != null) {
                    OnResultadoFinalizado();
               }
          }

          if (activeChoice == false) {
               //desativa os botões
               agoraBtn.gameObject.SetActive(false);
               depoisBtn.gameObject.SetActive(false);
          }
          else if (isEnemyActive == true) {
               if (activeChoice == true && attemptNumber == 1) {
                    // ativa só o botão imediato
                    agoraBtn.gameObject.SetActive(true);
                    depoisBtn.gameObject.SetActive(true);

                    agoraBtn.interactable = true;
                    depoisBtn.interactable = false;
               }
               else if (activeChoice == true && attemptNumber == 2) {
                    // ativa só botão atrasado
                    agoraBtn.gameObject.SetActive(true);
                    depoisBtn.gameObject.SetActive(true);

                    agoraBtn.interactable = false;
                    depoisBtn.interactable = true;
               }
               else if (activeChoice == true && attemptNumber > 2) {
                    //ativa ambos os botões
                    agoraBtn.gameObject.SetActive(true);
                    depoisBtn.gameObject.SetActive(true);

                    agoraBtn.interactable = true;
                    depoisBtn.interactable = true;
               }
          }      
     }

     public void escolhaImediataBtn () {
          // Escolha imediata
          selecaoAtual = ChoiceSelector.Imediata;
          isChosen = true; // informa que foi escolhido                    
          activeChoice = false; // desativa a permissão pra escolher
          resultadoFinalizado = true;
          // Debug.Log("Opção imediata foi escolhida. Causou " + imediateDamage + " de dano.");
     }

     public void escolhaAtrasadaBtn () {
          // Escolha atrasada
          selecaoAtual = ChoiceSelector.Atrasada;
          isChosen = true;
          activeChoice = false; // desativa a permissão pra escolher
          resultadoFinalizado = true;
          // Debug.Log("Opção atrasada foi escolhida. Causou " + delayedDamage + " de dano.");
     }


    public IEnumerator TimeForChoiceCoroutine(float choiceTime)
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
