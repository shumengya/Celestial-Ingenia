using UnityEngine;
using System.Collections.Generic;

public class BGMManager : MonoBehaviour
{
    public enum PlayMode
    {
        Loop,
        Random
    }

    public PlayMode playMode = PlayMode.Loop;
    public List<AudioClip> musicList;

    private AudioSource audioSource;
    private int currentIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = false;

        PlayNextMusic();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextMusic();
        }
    }

    void PlayNextMusic()
    {
        if (musicList.Count == 0)
        {
            return;
        }

        switch (playMode)
        {
            case PlayMode.Loop:
                currentIndex = (currentIndex + 1) % musicList.Count;
                break;
            case PlayMode.Random:
                currentIndex = Random.Range(0, musicList.Count);
                break;
        }

        audioSource.clip = musicList[currentIndex];
        audioSource.Play();
    }
}    