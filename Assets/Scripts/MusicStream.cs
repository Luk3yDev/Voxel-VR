using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStream : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip[] tracks;
    float timeUntilNextSong;
    int lastPlayedTrack;

    void Start()
    {
        QueueNextTrack(-60);
    }

    void Update()
    {
        timeUntilNextSong -= Time.deltaTime;
        if (timeUntilNextSong <= 0)
        {
            int newTrack = Random.Range(0, tracks.Length);
            while (newTrack == lastPlayedTrack)
            {
                newTrack = Random.Range(0, tracks.Length);
            }           
            AudioClip track = tracks[newTrack];
            musicSource.PlayOneShot(track, 0.6f);
            lastPlayedTrack = newTrack;
            QueueNextTrack(track.length);           
        }
    }

    void QueueNextTrack(float offset)
    {
        timeUntilNextSong = Random.Range(30, 60) + offset;
    }
}
