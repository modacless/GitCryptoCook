using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager AMInstance;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private bool musicSourceIsPlaying = false;

    [Header("SFX")]
    public AudioClip draxCardSFX;
    public AudioClip PickCardSFX;
    public AudioClip dropCardSFX;
    public AudioClip mouseOverCardSFX;


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


    public void PlayMusic(AudioClip musicClip)
    {
        musicSource.clip = musicClip;
        musicSource.Play();

        musicSourceIsPlaying = true;
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
