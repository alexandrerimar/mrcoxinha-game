using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Attempt : MonoBehaviour
{   
    public bool isChosen; //verifica se uma escolha foi feita
    public bool activeChoice = false; //permitido escolher?
    public int attemptNumber = 0; // 0 -> outras, 1 -> escolha 1, 2 -> escolha 2
    //public int imediateDamage = 10; // muda a cada fase
    //public int delayedDamage = 100; // constante
    //public int waitTime = 7; // alterado de acordo com escolhas em blocos
    

    // Variáveis de elapsed time
    private ComputeElapsedTime elapsedTimeClass;
    private ComputeElapsedTime elapsedTimeClassLatencia;
    public float escolhaImediataElapsedTime;
    public float latencia;

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

     //Referência a Enemy
     [SerializeField] GameObject enemy;
     private bool isEnemyActive;

     //Referência a Game Manager
     [SerializeField] GameObject controller;
     GameManager gameManagerScript;

     //Slider
     [SerializeField] Slider slider;


     void Start () {
          gameManagerScript = controller.GetComponent<GameManager>();
          elapsedTimeClass = new ComputeElapsedTime();
          elapsedTimeClassLatencia = new ComputeElapsedTime();

          agoraBtn.gameObject.SetActive(false);
          depoisBtn.gameObject.SetActive(false);
          slider.gameObject.SetActive(false);
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
          else if (isEnemyActive == true) 
          {

               if (gameManagerScript.sceneName == "EscolhaImpulsivaDD")
               {
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
               else if (gameManagerScript.sceneName == "InibicaoRespostaDD")
               {
                    agoraBtn.gameObject.SetActive(true);
                    depoisBtn.gameObject.SetActive(true);                    
                    
                    if (activeChoice == true && attemptNumber == 1) {
                         agoraBtn.interactable = true;
                         depoisBtn.interactable = false;
                    }
                    else if (activeChoice == true && attemptNumber == 2) {
                         agoraBtn.interactable = false;
                    }
                    
               } 
          }
          
     }

     public void escolhaImediataBtn () {
          // Escolha imediata
          selecaoAtual = ChoiceSelector.Imediata;
          escolhaImediataElapsedTime = elapsedTimeClass.ElapsedTime(inicio: false);
          latencia = elapsedTimeClassLatencia.ElapsedTime(inicio: false);
          Logger.Instance.LogAction("Latencia: " + latencia);
          isChosen = true; // informa que foi escolhido                    
          activeChoice = false; // desativa a permissão pra escolher
          resultadoFinalizado = true;
     }

     public void escolhaAtrasadaBtn () {
          // Escolha atrasada
          selecaoAtual = ChoiceSelector.Atrasada;
          latencia = elapsedTimeClassLatencia.ElapsedTime(inicio: false);
          Logger.Instance.LogAction("Latencia: " + latencia);
          isChosen = true;
          activeChoice = false; // desativa a permissão pra escolher
          resultadoFinalizado = true;
     }


    public IEnumerator TimeForChoiceCoroutine(float choiceTime)
    {
          latencia = elapsedTimeClassLatencia.ElapsedTime();
          Logger.Instance.LogAction("Latencia 0: " + latencia);
          yield return new WaitForSeconds(choiceTime);

          //Escolha não foi feita
          activeChoice = false; // desativa a permissão pra escolher
          selecaoAtual = ChoiceSelector.Nenhuma;
          resultadoFinalizado = true;
          Logger.Instance.LogAction("Tempo limite excedido, escolha não foi feita.");       
    }

     public IEnumerator TimeForChoiceCoroutineIRDD(float choiceTime, float timeForChoice)
     {
          latencia = elapsedTimeClassLatencia.ElapsedTime();
          Logger.Instance.LogAction("Latencia 0: " + latencia);

          agoraBtn.interactable = true;
          depoisBtn.interactable = false;
          if (attemptNumber > 1)
          {
               slider.gameObject.SetActive(true);
          }
          
          escolhaImediataElapsedTime = elapsedTimeClass.ElapsedTime(inicio: true);
          
          float timer = 0f;
          while (timer < choiceTime)
          {         
               // Calculate the progress based on the timer and choiceTime
               float progress = timer / choiceTime;
               // Set the slider value based on the progress
               slider.value = progress;
               // Update the timer
               timer += Time.deltaTime;
               yield return null;          
          }     

          latencia = elapsedTimeClassLatencia.ElapsedTime();
          Logger.Instance.LogAction("Latencia 0: " + latencia);        
          agoraBtn.interactable = false;
          depoisBtn.interactable = true;

          slider.gameObject.SetActive(false);

          yield return new WaitForSeconds(timeForChoice);

          activeChoice = false; // disable the choice
          selecaoAtual = ChoiceSelector.Nenhuma;
          resultadoFinalizado = true;
          Logger.Instance.LogAction("Tempo limite excedido, escolha não foi feita.");  
     }

/*
    public IEnumerator TimeForChoiceCoroutineIRDD(float choiceTime, float timeForChoice){
          // Espera o tempo determinado por choiceTime      
          // Choice time é deltaT: nesse caso, tempo para apresentação da opção atrasada.
          
          latencia = elapsedTimeClassLatencia.ElapsedTime();
          Logger.Instance.LogAction("Latencia 0: " + latencia);

          //agoraBtn.gameObject.SetActive(true);
          //depoisBtn.gameObject.SetActive(false);

          agoraBtn.interactable = true;
          depoisBtn.interactable = false;
          escolhaImediataElapsedTime = elapsedTimeClass.ElapsedTime(inicio: true);
          
          yield return new WaitForSeconds(choiceTime);

          latencia = elapsedTimeClassLatencia.ElapsedTime();
          Logger.Instance.LogAction("Latencia 0: " + latencia);        
          //agoraBtn.gameObject.SetActive(false);
          //depoisBtn.gameObject.SetActive(true);

          agoraBtn.interactable = false;
          depoisBtn.interactable = true;
          yield return new WaitForSeconds(timeForChoice);
          
          //Escolha não foi feita
          activeChoice = false; // desativa a permissão pra escolher
          selecaoAtual = ChoiceSelector.Nenhuma;
          resultadoFinalizado = true;
          Logger.Instance.LogAction("Tempo limite excedido, escolha não foi feita.");  
    }
*/

}
