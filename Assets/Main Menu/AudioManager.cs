using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

// This script manages audio settings for Master, Music, and Effects separately.
// It updates AudioMixer parameters, displays volume percentages, and plays click sounds on adjustments.
public class AudioSettingsManager : MonoBehaviour
{
    [Header("UI Sounds")]
    public AudioSource uiAudioSource; // Audio source for UI sound effects
    public AudioClip clickSound;      // Sound played when pressing volume buttons

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;     // Reference to the game's AudioMixer

    [Header("Value Texts")]
    public TextMeshProUGUI masterValueText;  // Displays Master volume percentage
    public TextMeshProUGUI musicValueText;   // Displays Music volume percentage
    public TextMeshProUGUI effectsValueText; // Displays Effects volume percentage

    // Internal volume values (0.0 to 1.0)
    private float masterVolume = 0.5f;
    private float musicVolume = 0.5f;
    private float effectsVolume = 0.5f;

    private void Start()
    {
        // Load previously saved volume settings or use default (50%) if none exist
        masterVolume = LoadVolume("MasterVolume", 0.5f);
        musicVolume = LoadVolume("MusicVolume", 0.5f);
        effectsVolume = LoadVolume("EffectsVolume", 0.5f);

        // Apply the loaded volumes to AudioMixer
        ApplyVolumes();
    }

    // Loads a saved volume value from PlayerPrefs, or returns default if not found
    private float LoadVolume(string key, float defaultValue)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
    }

    // Applies the internal volume variables into the AudioMixer
    private void ApplyVolumes()
    {
        SetMixerVolume("MasterVolume", masterVolume);
        SetMixerVolume("MusicVolume", musicVolume);
        SetMixerVolume("EffectsVolume", effectsVolume);
    }

    // Sets a specific exposed AudioMixer parameter to a given volume
    private void SetMixerVolume(string exposedParameter, float volume)
    {
        if (audioMixer != null)
        {
            if (volume <= 0.001f)
            {
                audioMixer.SetFloat(exposedParameter, -80f); // Full mute
            }
            else
            {
                audioMixer.SetFloat(exposedParameter, Mathf.Log10(volume) * 20); // Normal volume conversion
            }
        }

        // Save the new volume into PlayerPrefs
        PlayerPrefs.SetFloat(exposedParameter, volume);
    }

    // --- Volume Adjustment Functions for Buttons ---

    // Set Master Volume to a specific value (0%, 25%, 50%, 75%, 100%)
    public void SetMasterVolume(float value)
    {
        PlayClickSound();
        masterVolume = value;
        SetMixerVolume("MasterVolume", masterVolume);
    }

    // Set Music Volume to a specific value
    public void SetMusicVolume(float value)
    {
        PlayClickSound();
        musicVolume = value;
        SetMixerVolume("MusicVolume", musicVolume);
    }

    // Set Effects Volume to a specific value
    public void SetEffectsVolume(float value)
    {
        PlayClickSound();
        effectsVolume = value;
        SetMixerVolume("EffectsVolume", effectsVolume);
    }

    // Plays the UI button click sound
    private void PlayClickSound()
    {
        if (uiAudioSource && clickSound)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }

    // Manually save current volume settings to PlayerPrefs
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
        PlayerPrefs.Save();
        Debug.Log("Volume settings saved!");
    }

    public void ResetToDefaults()
{
    PlayClickSound(); // Optional: Play a click sound when resetting

    // Set volumes to default (50%)
    masterVolume = 0.5f;
    musicVolume = 0.5f;
    effectsVolume = 0.5f;

    // Apply the changes to AudioMixer
    audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
    audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
    audioMixer.SetFloat("EffectsVolume", Mathf.Log10(effectsVolume) * 20);

    // Save new defaults to PlayerPrefs
    PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
    PlayerPrefs.Save();
}

}
