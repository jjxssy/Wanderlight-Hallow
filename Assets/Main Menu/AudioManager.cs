using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages audio settings for Master, Music, and Effects separately.
/// Updates AudioMixer parameters, displays volume percentages,
/// plays UI click sounds, and saves settings via <see cref="PlayerPrefs"/>.
/// </summary>
public class AudioSettingsManager : MonoBehaviour
{
    [Header("UI Sounds")]
    /// <summary>
    /// Audio source used to play UI sound effects.
    /// </summary>
    public AudioSource uiAudioSource;

    /// <summary>
    /// Click sound played when pressing volume buttons.
    /// </summary>
    public AudioClip clickSound;

    [Header("Audio Mixer")]
    /// <summary>
    /// Reference to the game’s <see cref="AudioMixer"/>.
    /// </summary>
    public AudioMixer audioMixer;

    [Header("Value Texts")]
    /// <summary>
    /// Displays the Master volume percentage.
    /// </summary>
    public TextMeshProUGUI masterValueText;

    /// <summary>
    /// Displays the Music volume percentage.
    /// </summary>
    public TextMeshProUGUI musicValueText;

    /// <summary>
    /// Displays the Effects volume percentage.
    /// </summary>
    public TextMeshProUGUI effectsValueText;

    /// <summary>
    /// Current Master volume (0.0–1.0).
    /// </summary>
    private float masterVolume = 0.5f;

    /// <summary>
    /// Current Music volume (0.0–1.0).
    /// </summary>
    private float musicVolume = 0.5f;

    /// <summary>
    /// Current Effects volume (0.0–1.0).
    /// </summary>
    private float effectsVolume = 0.5f;

    /// <summary>
    /// Loads saved volume settings or applies defaults,
    /// then pushes values to the AudioMixer.
    /// </summary>
    private void Start()
    {
        masterVolume = LoadVolume("MasterVolume", 0.5f);
        musicVolume = LoadVolume("MusicVolume", 0.5f);
        effectsVolume = LoadVolume("EffectsVolume", 0.5f);

        ApplyVolumes();
    }

    /// <summary>
    /// Loads a saved volume value from <see cref="PlayerPrefs"/>.
    /// Returns default if no value is found.
    /// </summary>
    /// <param name="key">The PlayerPrefs key (e.g., "MasterVolume").</param>
    /// <param name="defaultValue">The fallback value if none is saved.</param>
    /// <returns>The saved or default volume (0.0–1.0).</returns>
    private float LoadVolume(string key, float defaultValue)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
    }

    /// <summary>
    /// Applies the current internal volume variables to the AudioMixer.
    /// </summary>
    private void ApplyVolumes()
    {
        SetMixerVolume("MasterVolume", masterVolume);
        SetMixerVolume("MusicVolume", musicVolume);
        SetMixerVolume("EffectsVolume", effectsVolume);
    }

    /// <summary>
    /// Sets a specific exposed AudioMixer parameter to a given volume.
    /// Also saves the value into <see cref="PlayerPrefs"/>.
    /// </summary>
    /// <param name="exposedParameter">The name of the exposed AudioMixer parameter.</param>
    /// <param name="volume">The volume level (0.0–1.0).</param>
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
                audioMixer.SetFloat(exposedParameter, Mathf.Log10(volume) * 20); // Convert linear to decibels
            }
        }

        PlayerPrefs.SetFloat(exposedParameter, volume);
    }

    // --- Volume Adjustment Functions for Buttons ---

    /// <summary>
    /// Sets the Master volume to a specific value.
    /// </summary>
    /// <param name="value">The new volume (0.0–1.0).</param>
    public void SetMasterVolume(float value)
    {
        PlayClickSound();
        masterVolume = value;
        SetMixerVolume("MasterVolume", masterVolume);
    }

    /// <summary>
    /// Sets the Music volume to a specific value.
    /// </summary>
    /// <param name="value">The new volume (0.0–1.0).</param>
    public void SetMusicVolume(float value)
    {
        PlayClickSound();
        musicVolume = value;
        SetMixerVolume("MusicVolume", musicVolume);
    }

    /// <summary>
    /// Sets the Effects volume to a specific value.
    /// </summary>
    /// <param name="value">The new volume (0.0–1.0).</param>
    public void SetEffectsVolume(float value)
    {
        PlayClickSound();
        effectsVolume = value;
        SetMixerVolume("EffectsVolume", effectsVolume);
    }

    /// <summary>
    /// Plays the UI button click sound.
    /// </summary>
    private void PlayClickSound()
    {
        if (uiAudioSource && clickSound)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }

    /// <summary>
    /// Saves the current volume settings to <see cref="PlayerPrefs"/>.
    /// </summary>
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Resets all volumes to defaults (50%) and saves them.
    /// Also plays a click sound if configured.
    /// </summary>
    public void ResetToDefaults()
    {
        PlayClickSound();

        masterVolume = 0.5f;
        musicVolume = 0.5f;
        effectsVolume = 0.5f;

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(effectsVolume) * 20);

        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);
        PlayerPrefs.Save();
    }
}
