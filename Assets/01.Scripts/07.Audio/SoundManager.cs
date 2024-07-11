using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SFX
{
    public string sfxName;
    public AudioClip sfxClip;
}

[System.Serializable]
public class Music
{
    public string musicName;
    public AudioClip musicClip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    [SerializeField] private bool BgmToggle;
    [SerializeField] private bool SfxToggle;

    public List<Music> musicList;
    public List<SFX> sfxList;

    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // Load settings when the game starts
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set slider values and add listeners
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            // Set slider value to match AudioSource volume
            musicVolumeSlider.value = musicSource.volume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            // Set slider value to match AudioSource volume
            sfxVolumeSlider.value = sfxSource.volume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    private void LoadSettings()
    {
        // Load volume and toggle settings
        float savedMusicVolume = PlayerPrefs.GetFloat("Slider", 1f); // Default volume 1.0f
        float savedSfxVolume = PlayerPrefs.GetFloat("Slider", 1f); // Default volume 1.0f
        bool savedBgmToggle = PlayerPrefs.GetInt("Toggle", 0) == 1; // Default false
        bool savedSfxToggle = PlayerPrefs.GetInt("Toggle", 0) == 1; // Default false

        // Apply saved settings
        musicSource.volume = savedMusicVolume;
        sfxSource.volume = savedSfxVolume;
        BgmToggle = savedBgmToggle;
        SfxToggle = savedSfxToggle;

        musicSource.mute = BgmToggle;
        sfxSource.mute = SfxToggle;

        // Ensure sliders match the loaded settings
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = savedMusicVolume;
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = savedSfxVolume;
        }
    }

    private void SaveSettings()
    {
        // Save volume and toggle settings
        PlayerPrefs.SetFloat("MusicVolume", musicSource.volume);
        PlayerPrefs.SetFloat("SfxVolume", sfxSource.volume);
        PlayerPrefs.SetInt("BgmToggle", BgmToggle ? 1 : 0);
        PlayerPrefs.SetInt("SfxToggle", SfxToggle ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        SaveSettings(); // Save volume when it's changed
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
        SaveSettings(); // Save volume when it's changed
    }

    public void ChangeBgmToggle()
    {
        BgmToggle = !BgmToggle;
        musicSource.mute = BgmToggle;
        SaveSettings(); // Save toggle setting when it's changed
    }

    public void ChangeSfxToggle()
    {
        SfxToggle = !SfxToggle;
        sfxSource.mute = SfxToggle;
        SaveSettings(); // Save toggle setting when it's changed
    }

    public void PlayMusic(string name)
    {
        foreach (Music music in musicList)
        {
            if (music.musicName == name)
            {
                musicSource.clip = music.musicClip;
                musicSource.Play();
                return;
            }
        }
        Debug.LogWarning("Music not found: " + name);
    }

    public void PlaySfx(string name)
    {
        foreach (SFX sfx in sfxList)
        {
            if (sfx.sfxName == name)
            {
                sfxSource.PlayOneShot(sfx.sfxClip);
                return;
            }
        }
        Debug.LogWarning("SFX not found: " + name);
    }
}
