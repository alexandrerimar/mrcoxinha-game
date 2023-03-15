using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public GameObject currentTarget;
    public bool lockOn;
    private Vector3 targetPos;

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

    public void SetTarget(GameObject target)
    {
        currentTarget = target;
    }

    public void LockOn()
    {
        lockOn = !lockOn;
    }
}
