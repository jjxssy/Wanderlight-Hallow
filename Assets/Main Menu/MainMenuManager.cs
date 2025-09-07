using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the main menu functionality such as starting a new game,
/// loading saved games, and exiting the application.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Save to Load")]
    /// <summary>
    /// The build index of the first playable game scene to load
    /// when starting a new game or loading a save.
    /// </summary>
    [SerializeField] private int firstGameSceneIndex;

    /// <summary>
    /// Dialog box shown when the player attempts to load a slot
    /// but no save is found.
    /// </summary>
    [SerializeField] private GameObject noSavesFoundDialog = null;

    /// <summary>
    /// Starts a new game by resetting the load index
    /// and loading the first game scene.
    /// </summary>
    public void NewGameDialog()
    {
        PlayerPrefs.SetInt("LoadIndex", 0);
        SceneManager.LoadScene(firstGameSceneIndex);
    }

    /// <summary>
    /// Attempts to load the first save slot (slot 1).
    /// If no save exists, displays <see cref="noSavesFoundDialog"/>.
    /// </summary>
    public void LoadGameDialog_Save1()
    {
        if (PlayerPrefs.HasKey("SavedLevel1"))
        {
            PlayerPrefs.SetInt("LoadIndex", 1);
            SceneManager.LoadScene(firstGameSceneIndex);
        }
        else
        {
            noSavesFoundDialog.SetActive(true);
        }
    }

    /// <summary>
    /// Attempts to load the second save slot (slot 2).
    /// If no save exists, displays <see cref="noSavesFoundDialog"/>.
    /// </summary>
    public void LoadGameDialog_Save2()
    {
        if (PlayerPrefs.HasKey("SavedLevel2"))
        {
            PlayerPrefs.SetInt("LoadIndex", 2);
            SceneManager.LoadScene(firstGameSceneIndex);
        }
        else
        {
            noSavesFoundDialog.SetActive(true);
        }
    }

    /// <summary>
    /// Attempts to load the third save slot (slot 3).
    /// If no save exists, displays <see cref="noSavesFoundDialog"/>.
    /// </summary>
    public void LoadGameDialog_Save3()
    {
        if (PlayerPrefs.HasKey("SavedLevel3"))
        {
            PlayerPrefs.SetInt("LoadIndex", 3);
            SceneManager.LoadScene(firstGameSceneIndex);
        }
        else
        {
            noSavesFoundDialog.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the "No Save Found" dialog box if visible.
    /// </summary>
    public void DisableNoSaveFoundDialog()
    {
        noSavesFoundDialog.SetActive(false);
    }

    /// <summary>
    /// Exits the application when the Exit button is pressed.
    /// </summary>
    public void ExitsButton()
    {
        Application.Quit();
    }
}
