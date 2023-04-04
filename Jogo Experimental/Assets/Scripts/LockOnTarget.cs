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

    // Variáveis para controle da rotação
    private bool isRotating;
    private bool isRotatingCamera;
    private float currentRotationTime;
    private float currentRotationTimeCamera;

    void Update()
    {
        if (currentTarget != null && currentTarget.activeSelf == true && lockOn)
        {
            // Desativa o movimento do personagem
            mainCamera.GetComponent<MouseLook>().enabled = false;
            GetComponent<MouseLook>().enabled = false;
            GetComponent<FPSInput>().enabled = false;

            // Verifica se o alvo está atrás do personagem e inverte a direção do vetor, se necessário
            if (Vector3.Dot(transform.forward, targetPosFromCamera) < 0)
            {
                targetPosFromCamera = -targetPosFromCamera;
            }

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

            // Determina quais eixos vão rotacionar (personagem)
            Vector3 newRotGoal = new Vector3(transform.rotation.x, rotGoal.y, transform.rotation.z);
            Quaternion newQuaternion = new Quaternion(newRotGoal.x, newRotGoal.y, newRotGoal.z, rotGoal.w);

            // Determina quais eixos vão rotacionar (camera)
            Vector3 newRotGoalCamera = new Vector3(rotGoal.x, rotGoal.y, rotGoal.z);
            Quaternion newQuaternionCamera = new Quaternion(newRotGoalCamera.x, newRotGoalCamera.y, newRotGoalCamera.z, rotGoal.w);

            // Rotaciona personagem
            if (!isRotating)
            {
                StartCoroutine(RotateTo(newQuaternion, speed));
            }

            // Rotaciona câmera
            if (!isRotatingCamera)
            {
                StartCoroutine(RotateToCamera(newQuaternionCamera, speedForCamera));
            }
        }
        else
        {
            // Ativa o movimento do personagem
            mainCamera.GetComponent<MouseLook>().enabled = true;
            GetComponent<MouseLook>().enabled = true;
            GetComponent<FPSInput>().enabled = true;
        }
    }

    // Rotaciona o personagem gradualmente
    IEnumerator RotateTo(Quaternion goalRotation, float speed)
    {
        isRotating = true;
        currentRotationTime = 0f;
        Quaternion startRotation = transform.rotation;

        while (currentRotationTime < rotationTime)
        {
            currentRotationTime += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(startRotation, goalRotation, currentRotationTime / rotationTime);

            yield return null;
        }

        isRotating = false;
    }

    // Rotaciona a câmera gradualmente
    IEnumerator RotateToCamera(Quaternion goalRotation, float speed) {
    isRotatingCamera = true;
    currentRotationTimeCamera = 0f;
    Quaternion startRotation = mainCamera.transform.rotation;

    while (currentRotationTimeCamera < rotationTime) {
        currentRotationTimeCamera += Time.deltaTime;
        mainCamera.transform.rotation = Quaternion.Slerp(startRotation, goalRotation, currentRotationTimeCamera / rotationTime);
        yield return null;
    }
    mainCamera.transform.rotation = goalRotation;
    isRotatingCamera = false;
    }
}