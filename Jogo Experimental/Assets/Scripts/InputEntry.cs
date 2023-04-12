using System;

[Serializable]
public class InputEntry {
    // Classe que cria o tipo de dado que será adicionado ao arquivo JSON

    public string etapaAtual; // Número de identificação da etapa: 'Sessão-Bloco-Tentativa'
    public float deltaTAtual; // Valor de atraso entre tentativas
    public string danoEscolhido; // Valor do dano escolhido: Imediato ou Atrasado (10).
    public int danoEscolhidoDummy; // É imediato? Ime = 1, Atr = 0, Nenhum = -1
    public string timeStamp; 
    public string playerID;

    public InputEntry (int[] etapaAtual, string danoEscolhido, float deltaTAtual, string timeStamp, string playerID = "None") { 
        this.etapaAtual = string.Join( "-", etapaAtual);
        this.danoEscolhido = danoEscolhido;
        this.deltaTAtual = deltaTAtual;
        this.timeStamp = timeStamp;
        this.playerID = playerID;

        switch (danoEscolhido) {
            case "Imediata":
                this.danoEscolhidoDummy = 1;
                break;
            case "Atrasada":
                this.danoEscolhidoDummy = 0;
                break;
            case "Nenhuma":
                this.danoEscolhidoDummy = -1;
                break;
        }
    }

}
