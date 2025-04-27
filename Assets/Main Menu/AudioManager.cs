using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro; // For using TextMeshPro Texts

// This script manages audio settings: master, music, and effects volume.
// It updates sliders, saves preferences, and displays volume values as percentages.
public class AudioSettingsManager : MonoBehaviour
{

    [Header("UI Sounds")]
    public AudioSource uiAudioSource;
    public AudioClip clickSound;


    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Reference to the game's AudioMixer

    [Header("Sliders")]
    public Slider masterSlider;  // Slider controlling master volume
    public Slider musicSlider;   // Slider controlling music volume
    public Slider effectsSlider; // Slider controlling effects (SFX) volume

    [Header("Value Texts")]
    public TextMeshProUGUI masterValueText;  // Text showing master volume %
    public TextMeshProUGUI musicValueText;   // Text showing music volume %
    public TextMeshProUGUI effectsValueText; // Text showing effects volume %

    private void Start()
{
    // Load each volume setting using the helper
    masterSlider.value = LoadVolume("MasterVolume", 0.5f);
    musicSlider.value = LoadVolume("MusicVolume", 0.5f);
    effectsSlider.value = LoadVolume("EffectsVolume", 0.5f);

    UpdateVolumeTexts();
    ApplyVolumes();
}

// Helper function to load a volume setting with a default fallback
private float LoadVolume(string key, float defaultValue)
{
    return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
}

    // Called when the Master Volume slider is moved
    public void OnMasterVolumeChanged(float volume)
    {
        // Update AudioMixer's MasterVolume parameter
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        // Save the value to PlayerPrefs for persistence
        PlayerPrefs.SetFloat("MasterVolume", volume);

        // Update the text to show the new volume in percentage
        masterValueText.text = (Mathf.RoundToInt(volume * 100)).ToString();
    }

    // Called when the Music Volume slider is moved
    public void OnMusicVolumeChanged(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        musicValueText.text = (Mathf.RoundToInt(volume * 100)).ToString();
    }

    // Called when the Effects Volume slider is moved
    public void OnEffectsVolumeChanged(float volume)
    {
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("EffectsVolume", volume);
        effectsValueText.text = (Mathf.RoundToInt(volume * 100)).ToString();
    }

    // Update all the volume value texts (used when loading settings)
    private void UpdateVolumeTexts()
    {
        masterValueText.text = (Mathf.RoundToInt(masterSlider.value * 100)).ToString();
        musicValueText.text = (Mathf.RoundToInt(musicSlider.value * 100)).ToString();
        effectsValueText.text = (Mathf.RoundToInt(effectsSlider.value * 100)).ToString();
    }

    // Apply the volumes from sliders into the AudioMixer (used at Start)
    private void ApplyVolumes()
    {
        OnMasterVolumeChanged(masterSlider.value);
        OnMusicVolumeChanged(musicSlider.value);
        OnEffectsVolumeChanged(effectsSlider.value);
    }

    public void ResetToDefaults()
{
    // Play click sound
    if (uiAudioSource && clickSound)
    {
        uiAudioSource.PlayOneShot(clickSound);
    }

    // Set default values
    float defaultVolume = 0.5f; // 50%

    masterSlider.value = defaultVolume;
    musicSlider.value = defaultVolume;
    effectsSlider.value = defaultVolume;

    // Apply immediately
    ApplyVolumes();

    // Save to PlayerPrefs
    PlayerPrefs.SetFloat("MasterVolume", defaultVolume);
    PlayerPrefs.SetFloat("MusicVolume", defaultVolume);
    PlayerPrefs.SetFloat("EffectsVolume", defaultVolume);

    // Update volume texts
    UpdateVolumeTexts();
}

public void SaveVolumeSettings()
{
    PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    PlayerPrefs.SetFloat("EffectsVolume", effectsSlider.value);

    PlayerPrefs.Save(); // Make sure data is written immediately

    Debug.Log("Volume settings saved!");
}


}
