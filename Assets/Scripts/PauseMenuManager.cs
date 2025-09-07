using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the in-game pause menu.
/// Allows pausing/resuming with ESC key and handles menu button actions:
/// Save, Settings, Achievements, Exit to Main Menu.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    /// <summary>
    /// The UI panel that contains the pause menu.
    /// </summary>
    [SerializeField] private GameObject pauseMenuUI;

    /// <summary>
    /// Button that triggers saving the game.
    /// </summary>
    [SerializeField] private Button saveButton;

    /// <summary>
    /// Button that opens the settings menu.
    /// </summary>
    [SerializeField] private Button settingsButton;

    /// <summary>
    /// Button that opens the achievements screen.
    /// </summary>
    [SerializeField] private Button achievementsButton;

    /// <summary>
    /// Button that exits to the main menu.
    /// </summary>
    [SerializeField] private Button exitButton;

    /// <summary>
    /// Tracks whether the game is currently paused.
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// Initializes button listeners and hides the pause menu on start.
    /// </summary>
    private void Start()
    {
        pauseMenuUI.SetActive(false);

        settingsButton.onClick.AddListener(OnSettingsClicked);
        achievementsButton.onClick.AddListener(OnAchievementsClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        PlayerPrefs.SetInt("MenusOpen", 0);
    }

    /// <summary>
    /// Checks for the Escape key press to toggle the pause menu.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else if(PlayerPrefs.GetInt("MenusOpen",0)==0)
                Pause();
        }
    }

    /// <summary>
    /// Activates the pause menu and freezes time.
    /// </summary>
    public void Pause()
    {
        PlayerPrefs.SetInt("MenusOpen", 1);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    /// <summary>
    /// Hides the pause menu and resumes time.
    /// </summary>
    public void Resume()
    {
        PlayerPrefs.SetInt("MenusOpen", 0);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    /// <summary>
    /// Called when the Settings button is clicked.
    /// You can open a settings panel or load a settings scene.
    /// </summary>
    private void OnSettingsClicked()
    {
        Debug.Log("Open Settings!");
    }

    /// <summary>
    /// Called when the Achievements button is clicked.
    /// Opens the achievements panel.
    /// </summary>
    private void OnAchievementsClicked()
    {
        Debug.Log("Open Achievements!");
    }

    /// <summary>
    /// Called when the Exit button is clicked.
    /// Resumes time and loads the Main Menu scene.
    /// </summary>
    private void OnExitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
