using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] GameObject painelPontos;
    private Score scoreScript;
    
    SoundManager soundManager;
    
    // Variáveis de spawn do inimigo
    [SerializeField] GameObject Player;
    public float globalMaxDistance, globalMinDistance;
    public GameObject Enemy;
    private float spawnDistance;

    Vector3 spawnPos;
    Vector3 VFXPos;

    //Tempo de espera inicial
    public float initialTimeWait;
    
    // Variaveis de controle de sessão, bloco e tentativa
    public int currentSession = 1;
    public int currentBlock = 1;
    public int currentAttempt = 1;
    public int totalSessions = 5;
    public int totalBlocks = 10; // pode variar de 03 a 20, precisa de script
    public int totalAttempts = 6;
    public float passos = 0.5f;

    // Valoes de dano para cada sessão
    public string DanoSessao1 = "10";
    public string DanoSessao2 = "30";
    public string DanoSessao3 = "50";
    public string DanoSessao4 = "70";
    public string DanoSessao5 = "90";

    // Variáveis de "Bloco"
    public float deltaT; // Tempo entre escolha e consequência (Esc.Imp). Te
    public float deltaTSuperior;
    public float deltaTInferior;
    public int totalDaEscolha; //soma das ultimas 4 tentativas do bloco
    List<float> lastThreeBlocks = new List<float>();
  

    //Variáveis da "Sessão"
    public string imediateDamage; // Primeiro dano imediato, muda a cada sessão
    public string delayedDamage = "100"; // Dano atrasado, contante
    public float pontoDeIndiferenca; // Guarda o ponto de indiferença
    //public int totalDeBlocos;

    // Variáveis de "Tentativas"
    public float deltaTInicial; // O primeiro deltaT de cada sessão
    public float IET; // Tempo de espera entre tentativas
    public float timeForChoice; //Tempo para fazer a escolha
    public float TTotalDaTentativa; // Tempo total da tentativa menos o tempo de escolha (IET + deltaT), estou ignorando tempo para resposta. Devo?
    public Attempt.ChoiceSelector selectedChoice;

    // Outros
    public Coroutine tempoDeEscolha = null; //Define uma variável para colocar a coroutine

    // Variáveis de apresentação de dano
    public GameObject damageTextPrefab;
    public string textToDisplay; 

    // Variáveis de Elapsed Time
    float tempoDaEscolhaIRDD;

    //Inicio de jogo
     [SerializeField] Button iniciarBtn;
     public bool gameStarted;

    public bool isScoreActivated;
    public bool isScoreByFase;
    ConfigManager configManager;

    void Start () { 
        configManager = GetComponent<ConfigManager>();        
        configManager.LoadConfigurations("config.conf"); 

        sceneName = GetActiveSceneName();

        Logger.Instance.LogAction("Cena: " + sceneName);
        Logger.Instance.LogAction("Total Sessions: " + totalSessions);
        Logger.Instance.LogAction("Total Blocks: " + totalBlocks);
        Logger.Instance.LogAction("Total Attempts: " + totalAttempts);
        Logger.Instance.LogAction("Passos: " + passos);
        Logger.Instance.LogAction("DeltaT Inicial: " + deltaTInicial);
        Logger.Instance.LogAction("DeltaT Inferior: " + deltaTInferior);
        Logger.Instance.LogAction("DeltaT Superior: " + deltaTSuperior);
        Logger.Instance.LogAction("Tempo total da tentativa: " + TTotalDaTentativa);
        Logger.Instance.LogAction("Tempo de Escolha: " + timeForChoice);
        Logger.Instance.LogAction("Dano maior: " + delayedDamage);

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        sceneLoaderScript = sceneLoader.GetComponent<SceneLoader>();        
        
        attemptScript = Player.GetComponent<Attempt>();
        animationScript = GetComponent<AnimationScript>();
        inputHandler = InputManager.GetComponent<InputHandler>();
        enemyVFXScript = Enemy.GetComponent<EnemyVFX>();
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        scoreScript = painelPontos.GetComponent<Score>(); 

        deltaT = deltaTInicial;

        // Desativa o inimigo no início da sessão
        Enemy.SetActive(false);
        spawnDistance = SetDistance();    

        Cursor.lockState = CursorLockMode.Confined;
        gameStarted = false;
        iniciarBtn.gameObject.SetActive(true);
    }  

    public void StartGame()
        {   
            gameStarted = true;
            iniciarBtn.gameObject.SetActive(false);
            Logger.Instance.LogAction("Jogo Iniciado");
            //Time.timeScale = 1; 
            Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine (WaitForStart());
        }

    void Update() {
        spawnDistance = SetDistance();

        if (isScoreActivated == true)
        {
            painelPontos.SetActive(true);
        }
        else if (isScoreActivated == false)
        {
            painelPontos.SetActive(false);
        }
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
        Logger.Instance.LogAction("Iniciando sessão " + currentSession);

        if (isScoreByFase == true)
        {
            scoreScript.ZerarScore();
        }

        switch (currentSession)
        {
            case 1:
                imediateDamage = DanoSessao1;
                break;
            case 2:
                imediateDamage = DanoSessao2;
                break;
            case 3:
                imediateDamage = DanoSessao3;
                break;
            case 4:
                imediateDamage = DanoSessao4;
                break;
            case 5:
                imediateDamage = DanoSessao5;
                break;
        }
    }

    public void StartBlock(int block)
    {
        currentBlock = block;
        Logger.Instance.LogAction("Iniciando bloco " + currentBlock);        
    }



    public void StartAttempt(int attempt){
        // Inicia a tentativa atual 
        Cursor.lockState = CursorLockMode.Confined;
        attemptScript.isChosen = false;
        currentAttempt = attempt;

        Logger.Instance.LogAction("Iniciando tentativa " + currentAttempt);

        // Spawna o inimigo no início da tentativa
        SpawnEnemy(spawnDistance);      
        Enemy.transform.LookAt(Player.transform);
        Logger.Instance.LogAction("Spawn do Inimigo. Distância: " + spawnDistance);

        attemptScript.attemptNumber = currentAttempt; // Informa a sessão atual para o script Attempt  

        attemptScript.activeChoice = true; // Permite fazer uma escolha         
        attemptScript.resultadoFinalizado = false; // Informa que a escolha ainda não foi feita           
        

        // Verifica se a escolha foi feita antes do tempo especificado
        if (sceneName == "EscolhaImpulsivaDD")
        {
            tempoDeEscolha = StartCoroutine(attemptScript.TimeForChoiceCoroutine(timeForChoice));
        } 
        else if (sceneName == "InibicaoRespostaDD")
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
        Cursor.lockState = CursorLockMode.Locked;
        //Screen.lockCursor = true;
        
        selectedChoice = attemptScript.selecaoAtual;

        switch (selectedChoice) 
        {
            case Attempt.ChoiceSelector.Imediata:
                Logger.Instance.LogAction("A escolha foi " + selectedChoice);                
                StopCoroutine(tempoDeEscolha);
                StartCoroutine(ConsequenciarEscolha(1));
                scoreScript.GetLastScore();
                break;
            case Attempt.ChoiceSelector.Atrasada:
                Logger.Instance.LogAction("A escolha foi " + selectedChoice);
                StopCoroutine(tempoDeEscolha);
                StartCoroutine(ConsequenciarEscolha(2));
                scoreScript.GetLastScore(false);
                break;
            case Attempt.ChoiceSelector.Nenhuma:
                Logger.Instance.LogAction("A escolha foi " + selectedChoice + ". Repetindo tentativa.");
                StartCoroutine(ConsequenciarEscolha(0));
                break;
        }          
        Attempt.OnResultadoFinalizado -= ComputarEscolha; // Desativa a função ComputarEscolha
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

                Logger.Instance.LogAction("Resultado da Sessão: Manteve no limite inferior");
                EndSession();
            }
            else if (lastThreeBlocks[0] == deltaTSuperior &&
                lastThreeBlocks[1] == deltaTSuperior &&
                lastThreeBlocks[2] == deltaTSuperior ) {

                pontoDeIndiferenca = ComputarPontoDeIndiferenca(); 

                Logger.Instance.LogAction("Resultado da Sessão: Manteve no limite superior");
                EndSession();
            }
            else if (lastThreeBlocks[0] == lastThreeBlocks[1] &&
                lastThreeBlocks[1] == lastThreeBlocks[2] &&
                lastThreeBlocks[2] == lastThreeBlocks[0] ) {

                pontoDeIndiferenca = ComputarPontoDeIndiferenca(); 

                Logger.Instance.LogAction("Resultado da Sessão: 3 blocos iguais. Manteve estabilidade.");
                EndSession();
            }    
            else if (currentBlock > totalBlocks) {
                
                pontoDeIndiferenca = ComputarPontoDeIndiferenca(true); 

                Logger.Instance.LogAction("Resultado da Sessão: Alcançou o máximo de blocos da sessão (" + totalBlocks + ")");
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
        Logger.Instance.LogAction("Soma das escolhas no bloco: " + total);
        totalDaEscolha = total;
    }


    private void AjustarIntervaloDeEspera() { 
        // Ajusta o deltaT de acordo com as escolhas dos jogadores nos 4 últimos blocos

        if (totalDaEscolha == 2) {
            // Mantém
            deltaT = deltaT;
            Logger.Instance.LogAction("DeltaT no bloco: mantido. Total das Escolhas = " + totalDaEscolha + ". E deltaT = " + deltaT);
        }
        else if (totalDaEscolha >=3 && totalDaEscolha < 5) {
            // Imediato
            float deltaTTemporarioImediato;
            deltaTTemporarioImediato = deltaT - passos;

            if (deltaTTemporarioImediato <= deltaTInferior) 
            {
                deltaT = deltaTInferior;
                Logger.Instance.LogAction("DeltaT no bloco: limite inferior. Total das Escolhas = " + totalDaEscolha + " E deltaT = " + deltaT);
            }
            else 
            { 
                deltaT = deltaTTemporarioImediato;
                Logger.Instance.LogAction("DeltaT no bloco: Computando bloco como 'Imediato'. Total das Escolhas = " + totalDaEscolha + " E deltaT = " + deltaT);
            }
            
        }
        else if (totalDaEscolha >= 0 && totalDaEscolha < 2) {
            // Atrasado
            float deltaTTemporarioAtrasado;
            deltaTTemporarioAtrasado = deltaT + passos;

            if (deltaTTemporarioAtrasado >= deltaTSuperior)
            {
                deltaT = deltaTSuperior;
                Logger.Instance.LogAction("DeltaT no bloco: limite superior. Total das Escolhas = " + totalDaEscolha + " E deltaT = " + deltaT);
            }
            else
            {
                deltaT = deltaTTemporarioAtrasado;
                Logger.Instance.LogAction("DeltaT no bloco: Computando bloco como 'Atrasado'. Total das Escolhas = " + totalDaEscolha + " E deltaT = " + deltaT);
            }
        }
        else {
            Logger.Instance.LogAction("Erro na função AjustarIntervalodeEspera " + totalDaEscolha);
        }
    }
    

    public void EndSession() {
        inputHandler.AddDataDaSessaoToList(); // Salva os pontos de indiferença no JSON

        currentSession++;

        if (currentSession > totalSessions) {
            Logger.Instance.LogAction("Todas as sessões foram finalizadas.");
            sceneLoaderScript.LoadSceneByPositionOnList(currentSceneIndex);    
        }
        else {
            currentBlock = 1;
            currentAttempt = 1;
            deltaT = deltaTInicial;
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
                Logger.Instance.LogAction("Jogador não escolheu: IET iniciado = " + IET);
                yield return new WaitForSeconds(IET);
                Logger.Instance.LogAction("Jogador não escolheu: IET finalizado = " + IET);
                
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(true);
            }
            else if (escolha == 1) {
                // Imediato

                IET = TTotalDaTentativa;

                Atirar(imediateDamage);
                Logger.Instance.LogAction("Jogador escolheu 'Imediato': IET iniciado = " + IET);
                yield return new WaitForSeconds(IET);
                Logger.Instance.LogAction("Jogador escolheu 'Imediato': IET finalizado = " + IET);

                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);

            }
            else if (escolha == 2) {
                // Atrasado 

                IET = TTotalDaTentativa - deltaT;

                Logger.Instance.LogAction("Jogador  escolheu 'Atrasado': deltaT iniciado = " + deltaT);
                yield return new WaitForSeconds(deltaT);
                Atirar(delayedDamage);
                Logger.Instance.LogAction("Jogador  escolheu 'Atrasado': deltaT finalizado = " + deltaT);
                
                Logger.Instance.LogAction("Jogador  escolheu 'Atrasado': IET Iniciado = " + IET);
                yield return new WaitForSeconds(IET);
                Logger.Instance.LogAction("Jogador  escolheu 'Atrasado': IET finalizado = " + IET);
                
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);                
            } 
            else {
                Logger.Instance.LogAction("Não foi passado argumento para a função AtivarIET()");
            }
        }
        else if (sceneName == "InibicaoRespostaDD")
        {
            if (escolha == 0) {
                // Nenhum          
                IET = TTotalDaTentativa - deltaT;  // Jogador já esperou e não escolheu, então removo deltaT, que já passou                
                NaoAtirar(); 
                Logger.Instance.LogAction("Jogador não escolheu: IET iniciado = " + IET);
                yield return new WaitForSeconds(IET);
                Logger.Instance.LogAction("Jogador não escolheu: IET finalizado = " + IET);               
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(true);
            }
            else if (escolha == 1) {
                // Imediato
                tempoDaEscolhaIRDD = attemptScript.escolhaImediataElapsedTime;
                
                if (tempoDaEscolhaIRDD < deltaT) 
                {
                    IET = TTotalDaTentativa - tempoDaEscolhaIRDD; 
                }
                else 
                {
                    IET = TTotalDaTentativa - deltaT;  
                }                
                Atirar(imediateDamage);
                Logger.Instance.LogAction("Jogador escolheu 'Imediato': IET iniciado = " + IET);
                yield return new WaitForSeconds(IET);
                Logger.Instance.LogAction("Jogador escolheu 'Imediato': IET finalizado = " + IET);
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);

            }
            else if (escolha == 2) {
                // Atrasado 
                IET = TTotalDaTentativa - deltaT;
                Atirar(delayedDamage);            
                Logger.Instance.LogAction("Jogador  escolheu 'Atrasado': IET Iniciado = " + IET);
                yield return new WaitForSeconds(IET);
                Logger.Instance.LogAction("Jogador  escolheu 'Atrasado': IET finalizado = " + IET);
                inputHandler.AddDataToList(); // Salva as escolhas do jogador no arquivo JSON
                EndAttempt(false);   
            } 
            else {
                Logger.Instance.LogAction("Não foi passado argumento para a função AtivarIET()");
            }
        }        
    }
}
