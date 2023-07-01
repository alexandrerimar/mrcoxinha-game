using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstespsScript : MonoBehaviour
{
    [SerializeField] GameObject footsteps;
    private FPSInput fpsInputScript;
    
    void Start()
    {
        fpsInputScript = gameObject.GetComponent<FPSInput>();
        footsteps.SetActive(false);
    }


    void Update()
    {
        if (fpsInputScript.isWalking == true) {
            footsteps.SetActive(true);
        }
        else {
            footsteps.SetActive(false);
        }
    }
}
