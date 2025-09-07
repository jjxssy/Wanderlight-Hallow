using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Manages screen resolution and display mode settings.
/// Provides dropdowns for resolution and screen mode selection,
/// applies and saves preferences with <see cref="PlayerPrefs"/>.
/// </summary>
public class GraphicsSettingsManager : MonoBehaviour
{
    /// <summary>
    /// Dropdown for selecting screen mode (Fullscreen, Windowed, Borderless).
    /// </summary>
    [SerializeField] private TMP_Dropdown screenDropdown;

    /// <summary>
    /// Dropdown for selecting screen resolution.
    /// </summary>
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    /// <summary>
    /// Button to reset graphics settings to default values.
    /// </summary>
    [SerializeField] private Button resetButton;

    /// <summary>
    /// List of available screen resolutions provided by Unity.
    /// </summary>
    private Resolution[] resolutions;

    /// <summary>
    /// Initializes dropdowns, loads saved preferences, or applies defaults on first run.
    /// </summary>
    private void Start()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefaults);

        // Populate screen mode dropdown
        screenDropdown.ClearOptions();
        screenDropdown.AddOptions(new System.Collections.Generic.List<string> {
            "FULLSCREEN", "WINDOWED", "BORDERLESS"
        });

        // Populate resolution dropdown
        resolutions = Screen.resolutions.Distinct().ToArray();
        resolutionDropdown.ClearOptions();
        var options = resolutions.Select(r => r.width + " x " + r.height).ToList();
        resolutionDropdown.AddOptions(options);

        if (!PlayerPrefs.HasKey("GraphicsInitialized"))
        {
            ResetToDefaults();  
            PlayerPrefs.SetInt("GraphicsInitialized", 1);
        }
        else
        {
            LoadSettings();
        }
    }

    /// <summary>
    /// Applies the current dropdown selections (resolution and screen mode)
    /// to the game screen and saves preferences to <see cref="PlayerPrefs"/>.
    /// </summary>
    public void ApplySettings()
    {
        Resolution selectedRes = resolutions[resolutionDropdown.value];
        FullScreenMode mode = GetScreenMode(screenDropdown.value);

        Screen.SetResolution(selectedRes.width, selectedRes.height, mode);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("ScreenMode", screenDropdown.value);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Resets graphics settings to defaults:
    /// Fullscreen at 2560x1440 resolution.
    /// </summary>
    public void ResetToDefaults()
    {
        screenDropdown.value = 0; // Fullscreen

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == 2560 && resolutions[i].height == 1440)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        ApplySettings();
    }

    /// <summary>
    /// Loads user preferences from <see cref="PlayerPrefs"/>
    /// and applies them to screen settings.
    /// </summary>
    private void LoadSettings()
    {
        int screenMode = PlayerPrefs.GetInt("ScreenMode", 0);
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);

        screenDropdown.value = screenMode;
        resolutionDropdown.value = Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1);

        ApplySettings();
    }

    /// <summary>
    /// Converts a dropdown index into Unity’s <see cref="FullScreenMode"/> enum.
    /// </summary>
    /// <param name="index">The index from the screen mode dropdown.</param>
    /// <returns>The matching <see cref="FullScreenMode"/>.</returns>
    private FullScreenMode GetScreenMode(int index)
    {
        return index switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.Windowed,
            2 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.ExclusiveFullScreen
        };
    }
}
