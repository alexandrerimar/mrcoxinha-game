using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField] GameObject controller;
    private GameManager gameManagerScript;
    [SerializeField] GameObject pontos;
    private TextMeshProUGUI pontosText;

    List<int> scoreList = new List<int>();
    public int lastScore;

    void Start()
    
    {
        gameManagerScript = controller.GetComponent<GameManager>(); 
        pontosText = pontos.GetComponent<TextMeshProUGUI>();
    }

    public void GetLastScore (bool isImediato = true)
    {
        string scoreStr;
        if (isImediato == true) {
            scoreStr = gameManagerScript.imediateDamage;
        }
        else
        {
            scoreStr = gameManagerScript.delayedDamage;
        }
        lastScore = int.Parse(scoreStr); 
        SetScore();      
    }
    
    public void SetScore ()
    {
        int pontuacaoTotal;

        scoreList.Add(lastScore);
        pontuacaoTotal = scoreList.Sum();   
        pontosText.text = pontuacaoTotal.ToString();
           
        Logger.Instance.LogAction("Pontos ganhos: " + lastScore); 
        Logger.Instance.LogAction("Pontuação Total: " + pontuacaoTotal); 
    }
}
