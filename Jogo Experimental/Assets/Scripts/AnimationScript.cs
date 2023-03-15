using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] private Animator shootAnim;
    private string shoot = "Shoot";

    public void Shoot () {
        shootAnim.Play(shoot, 0, 0.0f);
    }



}
