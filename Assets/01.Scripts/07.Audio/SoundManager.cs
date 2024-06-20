using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    

    public void ChangeBgmToggle()
    {
        BgmToggle = !BgmToggle;
        musicSource.mute = BgmToggle;
    }

    public void ChangeSfxToggle()
    {
        SfxToggle = !SfxToggle;
        sfxSource.mute = SfxToggle;
    }

    public void PlayMusic(string name)
    {
        foreach (Music music in musicList)
        {
            if (music.musicName == name)
            {
                musicSource.clip = music.musicClip;
                musicSource.Play();
            }
            else
            {
                Debug.Log("효과음을 찾지 못했습니다.");
            }
        }
    }

    public void PlaySfx(string name)
    {
        foreach(SFX sfx in sfxList)
        {
            if(sfx.sfxName == name)
            {
                sfxSource.PlayOneShot(sfx.sfxClip);
            }
            else
            {
                Debug.Log("효과음을 찾지 못했습니다.");
            }
        }
    }
    
}
