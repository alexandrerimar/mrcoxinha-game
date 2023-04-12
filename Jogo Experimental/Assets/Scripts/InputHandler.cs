using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputHandler : MonoBehaviour {
    // Player ID
    [SerializeField] string playerIDInput;
    
    // Variáveis de Tentativa
    [SerializeField] int[] etapaAtualInput;    
    [SerializeField] string danoEscolhidoInput;
    [SerializeField] float deltaTAtualInput;
    [SerializeField] string filename;

    // Variáveis de Bloco
    [SerializeField] int[] etapaAtualDeBlocoInput;
    [SerializeField] string filenameBloco;
    [SerializeField] int resultadoDoBlocoInput;
    [SerializeField] float deltaTDoBlocoInput;

    // Variáveis de Sessão
    [SerializeField] int etapaAtualDeSessaoInput;
    [SerializeField] string filenameSession;
    [SerializeField] float pontoDeIndiferencaInput;

    [SerializeField] GameObject controller;
    [SerializeField] GameObject mainMenu;
    private GameManager gameManager;

    List<InputEntry> entries = new List<InputEntry> ();
    List<BlocoEntry> blocoEntries = new List<BlocoEntry> ();
    List<SessionEntry> sessionEntries = new List<SessionEntry> ();

    void Awake() {
        playerIDInput = MainMenu.playerIDCrossScene; // Pega o ID do usuário no menu principal
        Debug.Log("Player Input passou: " + playerIDInput);
    }
    
    void Start () {
        gameManager = controller.GetComponent<GameManager>();
        entries = FileHandler.ReadListFromJSON<InputEntry> (filename);
        blocoEntries = FileHandler.ReadListFromJSON<BlocoEntry> (filenameBloco);
        sessionEntries = FileHandler.ReadListFromJSON<SessionEntry>(filenameSession);
    }   

    private void GetData () {
        // Cria variáveis com os dados da tentativa atual
        etapaAtualInput = new int[] {gameManager.currentSession, gameManager.currentBlock, gameManager.currentAttempt};
        danoEscolhidoInput = gameManager.selectedChoice.ToString();
        deltaTAtualInput = gameManager.deltaT;
    }

    private void GetDataBloco () {
        // Declara variáveis com os dados do bloco atual
        etapaAtualDeBlocoInput = new int[2] {gameManager.currentSession, gameManager.currentBlock};
        resultadoDoBlocoInput = gameManager.totalDaEscolha;
        deltaTDoBlocoInput = gameManager.deltaT;

    }

    private void GetDataSession () {
        // Declara variáveis com os dados da sessão atual
        etapaAtualDeSessaoInput = gameManager.currentSession;
        pontoDeIndiferencaInput = gameManager.pontoDeIndiferenca;
    }

    public void AddDataToList () {
        // Adiciona os dados da etapa atual no arquivo JSON
        GetData();
        string time;        
        time = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
        entries.Add (new InputEntry (etapaAtualInput, danoEscolhidoInput, deltaTAtualInput, time, playerIDInput));
        FileHandler.SaveToJSON<InputEntry>(entries, filename);        
    }

    public void AddDataDoBlocoToList () {
        // Adiciona os dados do bloco atual no arquivo JSON
        GetDataBloco();
        string time;
        time = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
        blocoEntries.Add (new BlocoEntry (etapaAtualDeBlocoInput[0], etapaAtualDeBlocoInput[1], resultadoDoBlocoInput, deltaTDoBlocoInput, time, playerIDInput));
        FileHandler.SaveToJSON<BlocoEntry>(blocoEntries, filenameBloco);
    }

    public void AddDataDaSessaoToList () {
        // Adiciona os dados da sessão no arquivo JSON
        GetDataSession();
        string time;
        time = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
        sessionEntries.Add (new SessionEntry (etapaAtualDeSessaoInput, pontoDeIndiferencaInput, time, playerIDInput));
        FileHandler.SaveToJSON<SessionEntry>(sessionEntries, filenameSession);
    }

    public List<float> GetLastThreeBlocks () {
        // Retorna uma lista com os três ultimos valores de DeltaT
        List<float> lastThreeDeltaT = new List<float>();
        

        for (int i = 1; i < 4; i++) {
            float input = blocoEntries[^i].deltaTDoBloco;
            lastThreeDeltaT.Add (input);    
        }
        foreach (float x in lastThreeDeltaT) {
            Debug.Log("itens da list " + x);
        }

        return lastThreeDeltaT; 
    }

    
    
    public int EncontrarIndex (string codigoDaTentativa) {
        // Encontra o index do item pesquisado na lista entries, com base no código da tentativa
        int index = entries.FindLastIndex(r => r.etapaAtual.Equals(codigoDaTentativa));

        if (index == -1) {
            Debug.Log("O item não foi encontrado");
            return index;
        }
        else {
            return index;
        }
    }
    
    private string ConverterParaCodigoDaTentativa(List<int> etapaAtual){
        // Converte a lista com (sessão, bloco, tentativa) para a string (sessão-bloco-tentativa)
        string codigo;
        codigo = string.Join("-", etapaAtual);
        return codigo;
    }

    public int ObterDanoEscolhidoNaTentativaDummy (List<int> codigo) {
        // Retorna o dano escolhido do participante na tentativa especificada

        string codigoConvertido;
        codigoConvertido = ConverterParaCodigoDaTentativa(codigo);

        int index;        
        index = EncontrarIndex(codigoConvertido);

        int dano;
        dano = entries[index].danoEscolhidoDummy;

        return dano;
    }
}
