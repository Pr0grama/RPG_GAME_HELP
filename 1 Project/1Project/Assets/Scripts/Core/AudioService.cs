using UnityEngine;
using UnityEngine.Audio;

public class AudioService : IService
{
    private AudioMixer audioMixer;
    private const string MusicVolumeParam = "MusicVolume";
    private float currentMusicVolume = 0.75f;

    public AudioService(AudioMixer mixer)
    {
        audioMixer = mixer;
    }

    public void Initialize()
    {
        LoadVolumeSettings();
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        audioMixer.SetFloat(MusicVolumeParam, Mathf.Log10(currentMusicVolume) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
    }

    public float GetMusicVolume()
    {
        return currentMusicVolume;
    }

    private void LoadVolumeSettings()
    {
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        SetMusicVolume(currentMusicVolume);
    }

    public void Cleanup() { }
}