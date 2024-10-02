using System.Collections;
using UnityEngine;

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
        if (Instance == null) Instance = this;
        else
        {
            this.bgmPlayer.volume = Instance.bgmPlayer.volume;
            this.sfxPlayer.volume = Instance.sfxPlayer.volume;
            Destroy(Instance.gameObject);
            Instance = this;
        }
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        StartCoroutine(StartMuteRoutine());
    }
    IEnumerator StartMuteRoutine()
    {
        Mute();
        yield return new WaitForSeconds(0.1f);
        UnMute();
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
    public void Mute()
    {
        bgmPlayer.mute = true;
        sfxPlayer.mute = true;
    }
    public void UnMute()
    {
        bgmPlayer.mute = false;
        sfxPlayer.mute = false;
    }
}
