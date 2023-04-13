using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] private Animator shootAnim;
    [SerializeField] private Animator enemyAnim;

    private string shoot = "Shoot";
    private string die = "Die";
    private string escape = "Escape";

    public void Shoot () {
        shootAnim.Play(shoot, 0, 0.0f);
    }

    public void Die () {
        enemyAnim.Play(die, 0, 0.0f);
    }

    public void Escape () {
        enemyAnim.Play(escape, 0, 0.0f);
    }



}
