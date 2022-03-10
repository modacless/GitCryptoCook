using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager AMInstance;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    [Header("SFX")]
    public AudioClip drawCardSFX;
    public AudioClip PickCardSFX;
    public AudioClip dropCardSFX;
    public AudioClip mouseOverCardSFX;

    [Header("UI")]
    public AudioClip nextTurnSFX;
    public AudioClip navigateSFX;
    public AudioClip readySFX;

    [Header("Music")]
    public AudioClip gameMusic;


    private void Awake()
    {
        if(AMInstance == null)
        {
            AMInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }



        DontDestroyOnLoad(this.gameObject);

        sfxSource = this.gameObject.AddComponent<AudioSource>();
        musicSource = this.gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;

    }//Instance

    private void Start()
    {
        SetMusicVolume(0.5f);
        PlayMusic(gameMusic);
    }


    public void PlayMusic(AudioClip musicClip)
    {
        if(!musicSource.isPlaying)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
            musicSource.clip = musicClip;
            musicSource.Play();
        }
        
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
