using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GraphicsSettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown screenDropdown;        // Dropdown to select screen mode (Fullscreen, Windowed, Borderless)
    [SerializeField] private TMP_Dropdown resolutionDropdown;    // Dropdown to select screen resolution
    [SerializeField] private Button resetButton;                 // Button to reset settings to default

    private Resolution[] resolutions;                            // List of available screen resolutions

    private void Start()
    {
        // Hook up the reset button
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefaults);

        // Populate screen mode dropdown options
        screenDropdown.ClearOptions();
        screenDropdown.AddOptions(new System.Collections.Generic.List<string> {
            "FULLSCREEN", "WINDOWED", "BORDERLESS"
        });

        // Get and populate resolution dropdown with available resolutions
        resolutions = Screen.resolutions.Distinct().ToArray();
        resolutionDropdown.ClearOptions();
        var options = resolutions.Select(r => r.width + " x " + r.height).ToList();
        resolutionDropdown.AddOptions(options);

        // If it's the user's first time running the game, apply default settings
        if (!PlayerPrefs.HasKey("GraphicsInitialized"))
        {
            ResetToDefaults();  // Apply default fullscreen 2560x1440
            PlayerPrefs.SetInt("GraphicsInitialized", 1); // Mark settings as initialized
        }
        else
        {
            LoadSettings(); // Load user-saved settings from PlayerPrefs
        }
    }

    // Apply the current dropdown selections to the game screen settings
    public void ApplySettings()
    {
        Resolution selectedRes = resolutions[resolutionDropdown.value];
        FullScreenMode mode = GetScreenMode(screenDropdown.value);

        // Apply resolution and screen mode
        Screen.SetResolution(selectedRes.width, selectedRes.height, mode);

        // Save current selections to PlayerPrefs
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("ScreenMode", screenDropdown.value);
        PlayerPrefs.Save();
    }

    // Reset screen settings to default: Fullscreen at 2560x1440
    public void ResetToDefaults()
    {
        screenDropdown.value = 0; // Set to Fullscreen (index 0)

        // Find and select the resolution 2560x1440 from the available options
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == 2560 && resolutions[i].height == 1440)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        ApplySettings(); // Apply the changes
    }

    // Load user preferences from PlayerPrefs and apply them
    private void LoadSettings()
    {
        int screenMode = PlayerPrefs.GetInt("ScreenMode", 0); // Default to fullscreen
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1); // Default to highest available

        // Safely assign dropdown values within valid range
        screenDropdown.value = screenMode;
        resolutionDropdown.value = Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1);

        ApplySettings(); // Apply loaded settings
    }

    // Helper method to convert dropdown index into Unity's FullScreenMode enum
    private FullScreenMode GetScreenMode(int index)
    {
        return index switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,  // Fullscreen
            1 => FullScreenMode.Windowed,              // Windowed
            2 => FullScreenMode.FullScreenWindow,      // Borderless
            _ => FullScreenMode.ExclusiveFullScreen    // Fallback
        };
    }
}
