using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public GameObject currentTarget;
    public bool lockOn;
    private Vector3 targetPos;
    private Vector3 targetPosFromCamera;
    Quaternion rotGoal;
    Quaternion rotGoalFromCamera;
    [SerializeField] float rotationTime = 0.75f;
    [SerializeField] private GameObject mainCamera;


    void Update() {
        if (currentTarget !=null && currentTarget.activeSelf == true && lockOn){
            // Desativa o movimento do personagem
            mainCamera.GetComponent<MouseLook>().enabled = false;
            GetComponent<MouseLook>().enabled = false;
            GetComponent<FPSInput>().enabled = false; 
          

            // Pega a posição do inimigo
            targetPos = (currentTarget.transform.position - transform.position).normalized;
            targetPosFromCamera = (currentTarget.transform.position - mainCamera.transform.position).normalized;
            rotGoal = Quaternion.LookRotation(targetPos);
            rotGoalFromCamera = Quaternion.LookRotation(targetPosFromCamera);

            // Calcula a velocidade de rotação
            float angle = Quaternion.Angle(transform.rotation, rotGoal);
            float angleFromCamera = Quaternion.Angle(mainCamera.transform.rotation, rotGoalFromCamera);
            
            float speed = angle / rotationTime;
            float speedForCamera = angleFromCamera / rotationTime;


            // Determina quais eixos vão rotacionas (personagem)
            Vector3 newRotGoal = new Vector3(transform.rotation.x, rotGoal.y, transform.rotation.z);
            Quaternion newQuaternion = new Quaternion(newRotGoal.x, newRotGoal.y, newRotGoal.z, rotGoal.w);            

            // Determina quais eixos vão rotacionas (camera)
            Vector3 newRotGoalCamera = new Vector3(rotGoal.x, rotGoal.y, rotGoal.z);
            Quaternion newQuaternionCamera = new Quaternion(newRotGoalCamera.x, newRotGoalCamera.y, newRotGoalCamera.z, rotGoal.w);
            

            // Rotaciona personagem
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newQuaternion, speed * Time.deltaTime);
            
            // Rotaciona câmera
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, newQuaternionCamera, speedForCamera * Time.deltaTime);

                    
        }
        else {
            // Ativa o movimento do personagem
            mainCamera.GetComponent<MouseLook>().enabled = true;
            GetComponent<MouseLook>().enabled = true;
            GetComponent<FPSInput>().enabled = true;
        }
    }
    
    Quaternion GetRotationGoal(Vector3 originPosition) {
        // Pega a posição do inimigo
        targetPos = (currentTarget.transform.position - originPosition).normalized;
        Quaternion rotGoal = Quaternion.LookRotation(targetPos);
        return(rotGoal);
    }
    
    
    public void SetTarget(GameObject target)
    {
        currentTarget = target;
    }
    

    public void LockOn()
    {
        lockOn = !lockOn;
    }

}    
    
    
    
    
    
    
    
    /*
    void Update()
    {
        if (currentTarget != null && currentTarget.activeSelf == true && lockOn)
        {
            // Desativa o movimento do personagem
            GetComponent<FPSInput>().enabled = false;
            GetComponentInChildren<MouseLook>().enabled = false;

            // Fixa a mira no alvo
            targetPos = currentTarget.transform.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
            transform.GetChild(0).transform.LookAt(targetPos);
        }
        else
        {
            // Ativa o movimento do personagem
            GetComponent<FPSInput>().enabled = true;
            GetComponentInChildren<MouseLook>().enabled = true;
        }
    }

    
    
    */

