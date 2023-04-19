using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private GameObject despawnVFX;

    void Start() {
        spawnVFX.SetActive(false);
        despawnVFX.SetActive(false);
    }

    public void ActivatEnemySpawnVFX(Vector3 position) {
        // Ativa o efeito de spawn
        spawnVFX.transform.position = position;
        spawnVFX.SetActive(true);
    }

    public void ActivateEnemyDespawnVFX(Vector3 position) {
        despawnVFX.transform.position = position;
        despawnVFX.SetActive(true);
    }
}
