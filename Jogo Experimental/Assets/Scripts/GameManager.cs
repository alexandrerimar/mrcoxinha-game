using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;


/*
PRÓXIMOS PASSOS
Importante:
[X] Organizar IET
[ ] Função para inimigo aparecer só quando personagem anda? Pois o player pode ficar só parado.

Depois:
1. Sonorizar: colocar som ambiente. 
*/

public class GameManager : MonoBehaviour
{
    public string sceneName;
    public int currentSceneIndex;
    
    // Dependências
    Attempt attemptScript;
    AnimationScript animationScript;
    private EnemyVFX enemyVFXScript;
    [SerializeField] GameObject InputManager;
    private InputHandler inputHandler;
    [SerializeField] GameObject canvasUI;
    [SerializeField] GameObject sceneLoader;
    SceneLoader sceneLoaderScript;
    
    SoundManager soundManager;
    
    // Variáveis de spawn do inimigo
    [SerializeField] GameObject Player;
    [SerializeField] float globalMaxDistance, globalMinDistance;
    public GameObject Enemy;
    private float spawnDistance;

    Vector3 spawnPos;
    Vector3 VFXPos;

    //Tempo de espera inicial
    [SerializeField] float initialTimeWait = 10f;
    
    // Variaveis de controle de sessão, bloco e tentativa
    public int currentSession = 1;
    public int currentBlock = 1;
    public int currentAttempt = 0;
    public int totalSessions = 5;
    public int totalBlocks = 20; // pode variar de 03 a 20, precisa de script
    public int totalAttempts = 6;

    // Variáveis de "Bloco"
    public float deltaT; // Tempo entre escolha e consequência (Esc.Imp). Te
    [SerializeField] float deltaTSuperior = 15.0f;
    [SerializeField] float deltaTInferior = 1.0f;
    public int totalDaEscolha; //soma das ultimas 4 tentativas do bloco
    List<float> lastThreeBlocks = new List<float>();
  

    //Variáveis da "Sessão"
    private string imediateDamage = "10"; // Primeiro dano imediato, muda a cada sessão
    public string delayedDamage = "100"; // Dano atrasado, contante
    public float pontoDeIndiferenca; // Guarda o ponto de indiferença
    //public int totalDeBlocos;

    // Variáveis de "Tentativas"
    public float deltaTInicial = 8.0f; // O primeiro deltaT de cada sessão
    public float IET; // Tempo de espera entre tentativas
    public float timeForChoice = 5.0f; //Tempo para fazer a escolha
    [SerializeField] int TTotalDaTentativa = 30; // Tempo total da tentativa menos o tempo de escolha (IET + deltaT), estou ignorando tempo para resposta. Devo?
    public Attempt.ChoiceSelector selectedChoice;

    // Outros
    public Coroutine tempoDeEscolha = null; //Define uma variável para colocar a coroutine

    // Variáveis de apresentação de dano
    public GameObject damageTextPrefab, camera;
    public string textToDisplay; 

    // Variáveis de Elapsed Time
    float tempoDaEscolhaIRDD;


    void Start() {         
        sceneName = GetActiveSceneName();
        Debug.Log("Cena: " + sceneName);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        sceneLoaderScript = sceneLoader.GetComponent<SceneLoader>();

        Screen.lockCursor = true; //esconde o cursor do mouse e não permite que saia da tela
        
        attemptScript = Player.GetComponent<Attempt>();
        animationScript = GetComponent<AnimationScript>();
        inputHandler = InputManager.GetComponent<InputHandler>();
        enemyVFXScript = Enemy.GetComponent<EnemyVFX>();
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

        deltaT = deltaTInicial;

        // Desativa o inimigo no início da sessão
        Enemy.SetActive(false);
        spawnDistance = SetDistance();
        
        // Inicia a primeira sessão, bloco e tentativa
        if (currentSession == 1 && currentBlock == 1 && currentAttempt == 1) {
            StartCoroutine (WaitForStart());
        } else {
            StartSession(currentSession);
            StartBlock(currentBlock);
            StartAttempt(currentAttempt);
        }  
    }  

    void Update() {
        spawnDistance = SetDistance();
    }

    private string GetActiveSceneName () {
        // Retorna o nome da Scene ativa atualmente;
        Scene currentScene = SceneManager.GetActiveScene();          
        string sceneName = currentScene.name;
        return sceneName;
    }

    IEnumerator WaitForStart () {
        // Espera um tempo antes de iniciar a primeira sessão
        yield return new WaitForSeconds(initialTimeWait);
        StartSession(currentSession);
        StartBlock(currentBlock);
        StartAttempt(currentAttempt);
    }


    public void SpawnEnemy (float distance) {
        // Spawna o inimigo
        if (Enemy.activeSelf == false) {
            Vector3 playerPos = Player.transform.position;
            Vector3 playerDirection = Player.transform.forward;
            Quaternion playerRotation = Player.transform.rotation;

            spawnPos = playerPos + playerDirection * distance;

            spawnPos.y = -1; 
            Enemy.transform.position = spawnPos;
            
            VFXPos = spawnPos;
            VFXPos.y = -0.85f;
            
            if (spawnDistance != 0) {
                Enemy.SetActive(true); 
                enemyVFXScript.ActivatEnemySpawnVFX(VFXPos);
                soundManager.PlaySFX(soundManager.enemySpawn);         
            }
        }
    }
   
    public float SetDistance () {
        // determina a distância em que o inimigo aparecerá
        Ray Ray = new Ray (Player.transform.position, Player.transform.forward);
        RaycastHit Hit; 
        Physics.SphereCast(Ray, 1.0f, out Hit);

        if (Hit.distance > globalMaxDistance){
            Hit.distance = globalMaxDistance;
        }
        
        float maxDistance = Hit.distance; 
        float minDistance = globalMinDistance;
        
        float distance = UnityEngine.Random.Range(minDistance, maxDistance);

       if (distance <= globalMinDistance) {
            distance = -globalMinDistance;
        }
        return distance;
    }  



    // Funções de controle das sessões, blocos e tentativas
    public void StartSession(int session)
    {
        currentSession = session;
        Debug.Log("Iniciando sessão " + currentSession);
        Logger.Instance.LogAction("Iniciando sessão " + currentSession);

        switch (currentSession)
        {
            case 1:
                imediateDamage = "10";
                break;
            case 2:
                imediateDamage = "30";
                break;
            case 3:
                imediateDamage = "50";
                break;
            case 4:
                imediateDamage = "70";
                break;
            case 5:
                imediateDamage = "90";
                break;
        }
    }

    public void StartBlock(int block)
    {
        currentBlock = block;
        Debug.Log("Iniciando bloco " + currentBlock);
    }



    public void StartAttempt(int attempt){
        // Inicia a tentativa atual
        Screen.lockCursor = false; 
        attemptScript.isChosen = false;
        currentAttempt = attempt;
        Debug.Log("Iniciando tentativa " + currentAttempt);

        // Spawna o inimigo no início da tentativa
        SpawnEnemy(spawnDistance);      
        Enemy.transform.LookAt(Player.transform);

        attemptScript.attemptNumber = currentAttempt; // Informa a sessão atual para o script Attempt  

        attemptScript.activeChoice = true; // Permite fazer uma escolha         
        attemptScript.resultadoFinalizado = false; // Informa que a escolha ainda não foi feita           
        

        // Verifica se a escolha foi feita antes do tempo especificado
        if (sceneName == "EscolhaImpulsivaDD")
        {
            tempoDeEscolha = StartCoroutine(attemptScript.TimeForChoiceCoroutine(timeForChoice));
        } else if (sceneName == "InibicaoRespostaDD")
        {            
            tempoDeEscolha = StartCoroutine(attemptScript.TimeForChoiceCoroutineIRDD(deltaT, timeForChoice));
        }
        
        // Chama a função 'ComputarEscolha' quando o resultado for finalizado
        Attempt.OnResultadoFinalizado += ComputarEscolha;
    }

    public void ComputarEscolha()
    {   
        // Atribui o valor da escolha para a variável selectedChoice
        // Computa escolha
        // Desativa o Evento OnResultadoFinalizado
        Screen.lockCursor = true;
        
        selectedChoice = attemptScript.selecaoAtual;

        switch (selectedChoice) 
        {
            case Attempt.ChoiceSelector.Imediata:
                Debug.Log("A escolha foi " + selectedChoice);                
                StopCoroutine(tempoDeEscolha);
                StartCoroutine(ConsequenciarEscolha(1));
                break;
            case Attempt.ChoiceSelector.Atrasada:
                Debug.Log("A escolha foi " + selectedChoice);
                StopCoroutine(tempoDeEscolha);
                StartCoroutine(ConsequenciarEscolha(2));
                break;
            case Attempt.ChoiceSelector.Nenhuma:
                Debug.Log("A escolha foi " + selectedChoice + ". Repetindo.");
                StartCoroutine(ConsequenciarEscolha(0));
                break;
        }
        
        
        Attempt.OnResultadoFinalizado -= ComputarEscolha; // Desativa a função ComputarEscolha
        //selectedChoice = Attempt.ChoiceSelector.Waiting;
    }
  

    public void EndAttempt(bool isRepeated) {
        bool condition = isRepeated;
        
        if (condition == true) {
            currentAttempt = currentAttempt;
        }
        else
        {
            currentAttempt++;
        }
       
        if (currentAttempt > totalAttempts)
        {
            EndBlock();
        }
        else
        {
            StartAttempt(currentAttempt);
        }
    }

    public void EndBlock() {
        ComputarBloco(); // Soma as 4 últimas tentativas
        inputHandler.AddDataDoBlocoToList(); // Salva dados do bloco no arquivo JSON 
        AjustarIntervaloDeEspera(); //Ajusta o intervalo do próximo bloco de acordo com as escolhas do anterior           
        
        currentBlock++;      

        if (currentBlock > 3) {
            lastThreeBlocks = inputHandler.GetLastThreeBlocks(); // Lista com os últimos 3 deltaT dos últimos 3 blocos
            
            if (lastThreeBlocks[0] == deltaTInferior &&
                lastThreeBlocks[1] == deltaTInferior &&
                lastThreeBlocks[2] == deltaTInferior ) {
                
                pontoDeIndiferenca = ComputarPontoDeIndiferenca(); 

                Debug.Log("Resultado da Sessão: Manteve no limite inferior");
                EndSession();
            }
            else if (lastThreeBlocks[0] == deltaTSuperior &&
                lastThreeBlocks[1] == deltaTSuperior &&
                lastThreeBlocks[2] == deltaTSuperior ) {

                pontoDeIndiferenca = ComputarPontoDeIndiferenca(); 

                Debug.Log("Resultado da Sessão: Manteve no limite superior");
                EndSession();
            }
            else if (lastThreeBlocks[0] == lastThreeBlocks[1] &&
                lastThreeBlocks[1] == lastThreeBlocks[2] &&
                lastThreeBlocks[2] == lastThreeBlocks[0] ) {

                pontoDeIndiferenca = ComputarPontoDeIndiferenca(); 

                Debug.Log("Resultado da Sessão: Houve 3 blocos iguais. Manteve estabilidade.");
                EndSession();
            }    
            else if (currentBlock > totalBlocks) {
                
                pontoDeIndiferenca = ComputarPontoDeIndiferenca(true); 

                Debug.Log("Resultado da Sessão: Alcançou o máximo de blocos da sessão");
                EndSession();
            }
            else {
                currentAttempt = 1;
                StartBlock(currentBlock);
                StartAttempt(currentAttempt);
            }   
        } 
        else {
            currentAttempt = 1;
            StartBlock(currentBlock);
            StartAttempt(currentAttempt);
        }  
    }

    private void ComputarBloco() {
        // Soma das 4 últimas tentativas do bloco
        List<int> codigoDaTentativa = new List<int>();
        List<int> escolhasNoBloco = new List<int>();
        codigoDaTentativa.Add(currentSession);
        codigoDaTentativa.Add(currentBlock);
        
        for (int i = 3; i < 7; i++) {
            codigoDaTentativa.Add(i); 
            int resultado;           
            
            resultado = inputHandler.ObterDanoEscolhidoNaTentativaDummy(codigoDaTentativa);

            escolhasNoBloco.Add(resultado); 
            codigoDaTentativa.RemoveAt(2);
        }  
        int total = escolhasNoBloco.Sum();
        //Debug.Log("total " + total);
        totalDaEscolha = total;
        //return total;
    }


    private void AjustarIntervaloDeEspera() { 
        // Ajusta o deltaT de acordo com as escolhas dos jogadores nos 4 últimos blocos
        //totalDaEscolha = ComputarBloco();
        //ComputarBloco();

        if (totalDaEscolha == 2) {
            // Mantém
            deltaT = deltaT;
            Debug.Log("Computando bloco como MANTÉM. totalDaEscolha = " + totalDaEscolha + " E deltaT é " + deltaT);
        }
        else if (totalDaEscolha >=3 && totalDaEscolha < 5) {
            // Imediato
            if (deltaT == deltaTInferior) {
                deltaT = deltaT;
                Debug.Log("Alcançou o limite inferior " + totalDaEscolha + " E deltaT é " + deltaT);
            }
            else {
            deltaT--;
            Debug.Log("Computando bloco como Imediato. totalDaEscolha = " + totalDaEscolha + " E deltaT é " + deltaT);
            }
            
        }
        else if (totalDaEscolha >= 0 && totalDaEscolha < 2) {
            // Atrasado
            if (deltaT == deltaTSuperior){
                deltaT = deltaT;
                Debug.Log("Alcançou o limite superior " + totalDaEscolha + " E deltaT é " + deltaT);
            }
            else{
                deltaT++;
                Debug.Log("Computando bloco como Atrasado. totalDaEscolha = " + totalDaEscolha + " E deltaT é " + deltaT);
            }
        }
        else {
            Debug.Log("Erro na função AjustarIntervalodeEspera " + totalDaEscolha);
        }
    }
    

    public void EndSession() {
        inputHandler.AddDataDaSessaoToList(); // Salva os pontos de indiferença no JSON

        currentSession++;

        if (currentSession > totalSessions) {
            Debug.Log("Todas as sessões foram finalizadas.");
            sceneLoaderScript.LoadSceneByPositionOnList(currentSceneIndex);    
        }
        else {
            currentBlock = 1;
            currentAttempt = 1;
            StartSession(currentSession);
            StartBlock(currentBlock);
            StartAttempt(currentAttempt);
        }
    }

    public float ComputarPontoDeIndiferenca(bool isMedia = false) {
        //Calcula o ponto de indiferença e salva no arquivo Json
        
        float media;
        float ponto;

        if (isMedia == false) {
            // O ponto de indiferença é o valor estável
            ponto = deltaT;
            return ponto;
        } 
        else {
            // O ponto de indiferença é a média dos últimos três valores
            media = (lastThreeBlocks[0] + lastThreeBlocks[1] + lastThreeBlocks[2])/3;
            
            ponto = media;
            return ponto;
        }        
    }

    public void Atirar(string danoCausado) {
        // Atira e mostra o dano causado
        
        textToDisplay = danoCausado; // Coleta a quantidade de dano
        soundManager.PlaySFX(soundManager.shot); // Ativa som de tiro
        animationScript.Shoot(); // Chama a função que ativa a animação de "Atirar"
        
        // Mostra a animação do texto de dano
        GameObject DamageTextInstance = Instantiate(damageTextPrefab, canvasUI.transform);
        DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(textToDisplay);

        KillEnemy(); // Tira o inimigo de cena
    }

    public void KillEnemy () {
        // Remove o inimigo de cena
        if (Enemy.activeSelf == true) {
            animationScript.Die();
            soundManager.PlaySFX(soundManager.enemyDie);
        }
    }

    public void NaoAtirar () {
        // Retira o inimigo de cena, sem atirar e mostra mensagem de que a escolha não foi feita.
        textToDisplay = "Inimigo escapou"; 

        GameObject DamageTextInstance = Instantiate(damageTextPrefab, canvasUI.transform);
        DamageTextInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(textToDisplay);

        EscapeEnemy (); // Tira o inimigo de cena
    }

    public void EscapeEnemy () {
        if (Enemy.activeSelf == true) {
            enemyVFXScript.ActivateEnemyDespawnVFX(VFXPos);
            animationScript.Escape();
            soundManager.PlaySFX(soundManager.enemySpawn);
        }
    }

    public IEnumerator ConsequenciarEscolha(int escolha) 
    {
        
        if (sceneName == "EscolhaImpulsivaDD")
        {
            if (escolha == 0) {
                // Nenhum        
                IET = TTotalDaTentativa;
                
                NaoAtirar(); 
                Debug.Log("Sem Escolha: IET Iniciado");
                yield return new WaitForSeconds(IET);
                Debug.Log("Sem Escolha: IET Finalizado");
                
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(true);
            }
            else if (escolha == 1) {
                // Imediato

                IET = TTotalDaTentativa;

                Atirar(imediateDamage);
                Debug.Log("Escolha Imediata: IET Iniciado");
                yield return new WaitForSeconds(IET);
                Debug.Log("Escolha Imediata: IET Finalizado");

                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);

            }
            else if (escolha == 2) {
                // Atrasado 

                IET = TTotalDaTentativa - deltaT;

                Debug.Log("Escolha Atrasada: deltaT Iniciado");
                yield return new WaitForSeconds(deltaT);
                Atirar(delayedDamage);
                Debug.Log("Escolha Atrasada: deltaT Finalizado");
                
                Debug.Log("Escolha Atrasada: IET Iniciado");
                yield return new WaitForSeconds(IET);
                Debug.Log("Escolha Atrasada: IET Finalizado");
                
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);                
            } 
            else {
                Debug.Log("Não foi passado argumento para a função AtivarIET()");
            }
        }
        else if (sceneName == "InibicaoRespostaDD")
        {
            if (escolha == 0) {
                // Nenhum          
                IET = TTotalDaTentativa - deltaT;  // Jogador já esperou e não escolheu, então removo deltaT, que já passou                
                NaoAtirar(); 
                Debug.Log("Sem Escolha: IET Iniciado");
                yield return new WaitForSeconds(IET);
                Debug.Log("Sem Escolha: IET Finalizado");                
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(true);
            }
            else if (escolha == 1) {
                // Imediato
                tempoDaEscolhaIRDD = Convert.ToSingle(attemptScript.elapsedTime.TotalSeconds);
                //tempoDaEscolhaIRDD = attemptScript.elapsedTime.TotalSeconds;
                Debug.Log("Convertido " + tempoDaEscolhaIRDD);

                if (tempoDaEscolhaIRDD < deltaT) 
                {
                    IET = TTotalDaTentativa - tempoDaEscolhaIRDD; 
                }
                else 
                {
                    IET = TTotalDaTentativa - deltaT;  
                }                
                Atirar(imediateDamage);
                Debug.Log("Escolha Imediata: IET Iniciado");
                yield return new WaitForSeconds(IET);
                Debug.Log("Escolha Imediata: IET Finalizado");
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);

            }
            else if (escolha == 2) {
                // Atrasado 
                IET = TTotalDaTentativa - deltaT;
                Atirar(delayedDamage);            
                Debug.Log("Escolha Atrasada: IET Iniciado");
                yield return new WaitForSeconds(IET);
                Debug.Log("Escolha Atrasada: IET Finalizado");
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);   
            } 
            else {
                Debug.Log("Não foi passado argumento para a função AtivarIET()");
            }
        }        
    }
}
