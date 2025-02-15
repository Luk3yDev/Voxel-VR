using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundKiller : MonoBehaviour
{
    [SerializeField] float volume = 0.8f;
    void Start()
    {
        AudioClip audioClip = GetComponent<AudioSource>().clip;
        GetComponent<AudioSource>().PlayOneShot(audioClip, volume);
        Destroy(gameObject, audioClip.length);
    }
}
