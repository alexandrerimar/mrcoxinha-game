using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnTargetCamera : MonoBehaviour
{
    public GameObject currentTarget;
    public bool lockOn;
    private Vector3 targetPos;
    Quaternion rotGoal;
    [SerializeField] float rotationTime = 0.75f;
    [SerializeField] GameObject player;
    LockOnTarget playerScript;
   

    void Start () {
        playerScript = player.GetComponent<LockOnTarget>();
    }


    void Update() {
        if (currentTarget !=null && currentTarget.activeSelf == true && playerScript.lockOn == true){
            // Desativa o movimento do personagem
            GetComponent<MouseLook>().enabled = false;
          

            // Pega a posição do inimigo
            targetPos = (currentTarget.transform.position - transform.position).normalized;
            rotGoal = Quaternion.LookRotation(targetPos);
            

            // Calcula a velocidade de rotação
            float angle = Quaternion.Angle(transform.rotation, rotGoal);
            float speed = angle / rotationTime;


            // Determina quais eixos vão rotacionas (personagem)
            Vector3 newRotGoal = new Vector3(rotGoal.x, 0, 0);
            Quaternion newQuaternion = new Quaternion(newRotGoal.x, newRotGoal.y, newRotGoal.z, rotGoal.w);            
            
            // Rotaciona personagem
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newQuaternion, speed * Time.deltaTime);


                    
        }
        else {
            // Ativa o movimento do personagem
            GetComponent<MouseLook>().enabled = true;
        }
    }
    

    public void LockOn()
    {
        lockOn = !lockOn;
    }

} 