using System;

[Serializable]
public class SessionEntry {
    // Clase que define o tipo de dado salvo para a sessão

    public int sessao; // Qual a sessão em questão
    public float pontoDeIndiferenca; // Ponto de indiferença da sessão
    public string timeStamp; // Tempo em que o dado foi anotado
    public string playerID; //

    public SessionEntry (int sessao, float pontoDeIndiferenca, string timeStamp, string playerID = "None") {
        this.sessao = sessao;
        this.pontoDeIndiferenca = pontoDeIndiferenca;
        this.timeStamp = timeStamp;
        this.playerID = playerID;
    }
}
