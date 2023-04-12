using System;

[Serializable]
public class BlocoEntry {
    // Classe que define o tipo de dado salvo para o bloco

    public string etapaAtualDeBloco; //Sessão-Bloco
    public int resultadoDoBloco; //Soma das ultimas 4 etapas do bloco
    public float deltaTDoBloco; // O deltaT usado nesse bloco
    public string timeStamp; //tempo em que o dado foi anotado
    public string playerID;

    public BlocoEntry (int sessaoAtual, int blocoAtual, int resultadoDoBloco, float deltaTDoBloco, string timeStamp, string playerID = "None") {
        this.etapaAtualDeBloco = sessaoAtual.ToString() + "-" + blocoAtual.ToString();
        this.resultadoDoBloco = resultadoDoBloco;
        this.deltaTDoBloco = deltaTDoBloco;
        this.timeStamp = timeStamp;
        this.playerID = playerID;
    }
}
