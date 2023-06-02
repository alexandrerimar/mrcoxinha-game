using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]

public class FPSInput : MonoBehaviour
{
    public float speed = 6.0f; 
    public float gravity = -9.8f;
    float deltaX;
    float deltaZ ;

    private CharacterController charController;
    public bool isWalking;

    private LockOnTarget lockOnTargetScript;

    [SerializeField] GameObject controller;
    private GameManager gameManagerScript;

    void Start() 
    {
      charController = GetComponent<CharacterController>();
      lockOnTargetScript = GetComponent<LockOnTarget>();
      gameManagerScript = controller.GetComponent<GameManager>();
    }

    void Update()
    {
      if (gameManagerScript.gameStarted == true)
      {
        if (lockOnTargetScript.isLocked == false) {
          deltaX = Input.GetAxis("Horizontal") * speed;
          deltaZ = Input.GetAxis("Vertical") * speed;
        }
        else {
          deltaX = 0;
          deltaZ = 0;
        }
      }
      Vector3 movement = new Vector3(deltaX, 0, deltaZ);
      movement = Vector3.ClampMagnitude(movement, speed);
      movement.y = gravity;
      movement *= Time.deltaTime;
      movement = transform.TransformDirection(movement); // Tranforma as coordenadas locais em globais, para passar para como argumento para Move
      
      charController.Move(movement); 

      if (deltaX != 0 || deltaZ != 0) {
        isWalking = true;
      } else if (deltaX == 0 || deltaZ == 0){
        isWalking = false;
      }

    }
}
