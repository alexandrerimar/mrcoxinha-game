using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    [SerializeField] private GameObject spawnVFX;

    void Start() {
        spawnVFX.SetActive(false);
    }

    public void ActivatEnemySpawnVFX(Vector3 position) {
        // Ativa o efeito de spawn
        spawnVFX.transform.position = position;
        spawnVFX.SetActive(true);
    }
}
