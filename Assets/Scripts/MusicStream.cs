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
        // Play a song on start with zero delay
        QueueNextTrack(0);
    }

    void Update()
    {
        timeUntilNextSong -= Time.deltaTime;
        if (timeUntilNextSong <= 0)
        {
            int newTrack = Random.Range(0, tracks.Length);
            if (newTrack == lastPlayedTrack)
            {
                // Reroll for a different track
                QueueNextTrack(0);
            }

            AudioClip track = tracks[newTrack];
            musicSource.PlayOneShot(track, 0.7f);
            QueueNextTrack(track.length);
        }
    }

    void QueueNextTrack(float offset)
    {
        timeUntilNextSong = Random.Range(30, 60) + offset;
    }
}
