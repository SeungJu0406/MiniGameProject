using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmPlayer;
    public AudioSource sfxPlayer;

    [System.Serializable]
    public struct BGM
    {
        public AudioClip title;
        public AudioClip[] game;
    }
    [System.Serializable]
    public struct SFX
    {
        public AudioClip eat;
        public AudioClip cell;
        public AudioClip settle;
        public AudioClip attack;
        public AudioClip unclick;
        public AudioClip combine;      
        public AudioClip recipeUI;
        public AudioClip UIButton;
    }

    public BGM bgm;
    public SFX sfx;

    private void Awake()
    {
        if (Instance == null)Instance = this;
        
        else Destroy(gameObject);
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmPlayer.clip = clip;
        bgmPlayer.Play();
    }
    public void StopBgm()
    {
        if (bgmPlayer.isPlaying)
        {
            bgmPlayer.Stop();
        }
    }
    public void PauseBGM()
    {
        if (bgmPlayer.isPlaying) 
        {
            bgmPlayer.Pause();
        }
    }
    public void SetBGM(float volume)
    {
        bgmPlayer.volume = volume;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxPlayer.PlayOneShot(clip);
    }
    public void SetSFX(float volume)
    {
        sfxPlayer.volume = volume;
    }
}
