using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    //싱글톤
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Music");
    }

    /// <summary>
    /// 음악을 실행시키는 함수
    /// </summary>
    /// <param name="name">음악의 이름</param>
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Music Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    /// <summary>
    /// 음악을 멈추는 함수
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    /// <summary>
    /// 효과음을 실행시키는 함수
    /// </summary>
    /// <param name="name">효과음의 이름</param>
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sount Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    /// <summary>
    /// 음악의 소리를 조절하는 함수
    /// </summary>
    /// <param name="volume"></param>
    /// <param name="audioMixer"></param>
    public void SetMusicVolume(float volume, AudioMixer audioMixer)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("Music", -80f);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    /// <summary>
    /// 효과음의 소리를 조절하는 함수
    /// </summary>
    /// <param name="volume"></param>
    /// <param name="audioMixer"></param>
    public void SetSFXVolume(float volume, AudioMixer audioMixer)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    /// <summary>
    /// 소리를 조절한 값을 받아와서 설정해주는 함수
    /// </summary>
    /// <param name="musicVolume"></param>
    /// <param name="sfxVolume"></param>
    /// <param name="audioMixer"></param>
    public void LoadVolume(float musicVolume, float sfxVolume, AudioMixer audioMixer)
    {
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");

        SetMusicVolume(musicVolume, audioMixer);
        SetSFXVolume(sfxVolume, audioMixer);
    }

}

