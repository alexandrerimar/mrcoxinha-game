using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{ 
    [SerializeField] GameObject Player;
    [SerializeField] float globalMaxDistance, globalMinDistance;
    public int IET = 2; //tempo de espera entre tentativas (após inimigo desaparecer)
    //bool verifyPos;
    
    //public Transform player; 
    public GameObject Enemy;
    private float spawnDistance;

  
    void Start () {
        Enemy.SetActive(false);
        //if (Enemy == null){
            //Enemy = Instantiate(SlimePBR) as GameObject;
            //Enemy.SetActive(false);
       // }
    }
    
    void Update() {  
        spawnDistance = SetDistance();
        StartCoroutine(SpawnEnemy(spawnDistance));      
        //player = Player.transform;
        Enemy.transform.LookAt(Player.transform);         
    }

    IEnumerator SpawnEnemy (float distance) {     
        // Spawna o inimigo na frente do jogador, durante um intervalo. Em seguida, esconde o inimigo por mais um tempo.

        if (Enemy.activeSelf == false) {
            Vector3 playerPos = Player.transform.position;
            Vector3 playerDirection = Player.transform.forward;
            Quaternion playerRotation = Player.transform.rotation;

            Vector3 spawnPos = playerPos + playerDirection * distance;

            spawnPos.y = -1; 
            Enemy.transform.position = spawnPos;         
          
            if (spawnDistance != 0) {
                //yield return new WaitForSeconds(1); //tempo entre apresentações
                Enemy.SetActive(true);
                yield return new WaitForSeconds(10); //tempo que o inimigo aparece
                Enemy.SetActive(false);
            }
        }
   }
   
    public float SetDistance () {
        Ray Ray = new Ray (Player.transform.position, Player.transform.forward);
        RaycastHit Hit; 
        Physics.SphereCast(Ray, 1.0f, out Hit);

        if (Hit.distance > globalMaxDistance){
            Hit.distance = globalMaxDistance;
        }
        
        float maxDistance = Hit.distance; 
        float minDistance = globalMinDistance;
        
        float distance = Random.Range(minDistance, maxDistance);

       if (distance <= globalMinDistance) {
            distance = 0;
        }
        return distance;
    }  
}