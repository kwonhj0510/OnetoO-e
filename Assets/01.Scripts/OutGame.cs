using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OutGame : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private Slider musicSlider = null;
    [SerializeField] private Slider sfxSlider = null;

    /// <summary>
    /// 소리조절 슬라이더 초기화
    /// </summary>
    private void Start()
    {
        musicSlider.onValueChanged.AddListener((value) => SoundManager.instance.SetMusicVolume(value, audioMixer));
        sfxSlider.onValueChanged.AddListener((value) => SoundManager.instance.SetSFXVolume(value, audioMixer));


        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("SFXVolume"))
        {
            SoundManager.instance.LoadVolume(musicSlider.value, sfxSlider.value, audioMixer);

            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        }
        else
        {
            SoundManager.instance.SetMusicVolume(musicSlider.value, audioMixer);
            SoundManager.instance.SetSFXVolume(sfxSlider.value, audioMixer);
        }
    }
}
