using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateEnemy : MonoBehaviour
{
    public void DeactivateEnemyOnAnimationEnd() {
        gameObject.SetActive(false);
    }
}
