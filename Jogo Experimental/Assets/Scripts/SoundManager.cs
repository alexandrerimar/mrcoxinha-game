using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I;
    
    [Header("-- Audio Source --")]

    [SerializeField] private AudioSource _SFXSource;
    [SerializeField] private AudioSource _MusicSource;
    [SerializeField] private AudioSource _WalkingSource;
    
    [Header("-- Audio Clips --")]
    [SerializeField] public AudioClip background;
    [SerializeField] public AudioClip shot;
    [SerializeField] public AudioClip enemySpawn;
    [SerializeField] public AudioClip enemyDie;
    [SerializeField] public AudioClip walkingSound;


    void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }        
    }

    public void PlaySFX(AudioClip clip) {
        _SFXSource.PlayOneShot(clip);
    }

    public void PlayWalkingSound() {
        _WalkingSource.clip = walkingSound;
        _WalkingSource.Play();
    }

}
