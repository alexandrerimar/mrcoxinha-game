using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;
    
    public float sensitivityHor = 9.0f; 
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    
    private float verticalRot;

    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody> ();
        if (body != null) {
            body.freezeRotation = true;
        }
    }

    void Update()
    {
        if (axes == RotationAxes.MouseX) {
            //Controla a rotacao horizontal
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
        }
        else if (axes == RotationAxes.MouseY) {
            //Controla a rotacao vertical
            verticalRot -= Input.GetAxis("Mouse Y") * sensitivityVert;
            verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);
            //Limita a rotacao vertical em - 45º e +45º

            float horizontalRot = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3 (verticalRot, horizontalRot, 0);
            //Cria um Vector3, porque não dá pra alterar os valores de transform, aí copia pra uma nova variável.
            //Usa EulerAngles, para não precisa usar Quartenios
        }
        else {
            //Rotação vertical e horizontal
            verticalRot -= Input.GetAxis("Mouse Y") * sensitivityVert;
            verticalRot = Mathf.Clamp(verticalRot, minimumVert, maximumVert);

            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float horizontalRot = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3 (verticalRot, horizontalRot, 0);
        }
    }
}
