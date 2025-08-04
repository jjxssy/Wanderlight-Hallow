using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Manages the main menu functionality such as starting a new game, loading saves, and exiting the game
public class MainMenuManager : MonoBehaviour
{

    [Header("Save to Load")]
    [SerializeField] private int firstGameSceneIndex;
    [SerializeField] private GameObject noSavesFoundDialog = null; // Dialog box to show when no saved games are found

    // Called when the player chooses "New Game" - loads the new game scene
    public void NewGameDialog()
    {
        PlayerPrefs.SetInt("LoadIndex", 0);
        SceneManager.LoadScene(firstGameSceneIndex);
    }

    // Called when the player selects the first save slot
    public void LoadGameDialog_Save1()
    {
        if (PlayerPrefs.HasKey("SavedLevel1")) // Check if a save exists in slot 1
        {
            PlayerPrefs.SetInt("LoadIndex", 1);
            SceneManager.LoadScene(firstGameSceneIndex);
        }
        else
        {
            noSavesFoundDialog.SetActive(true); // Show dialog if no save found
        }
    }

    // Called when the player selects the second save slot
    public void LoadGameDialog_Save2()
    {
        if (PlayerPrefs.HasKey("SavedLevel2")) // Check if a save exists in slot 2
        {
            PlayerPrefs.SetInt("LoadIndex", 2);
            SceneManager.LoadScene(firstGameSceneIndex);
        }
        else
        {
            noSavesFoundDialog.SetActive(true); // Show dialog if no save found
        }
    }

    // Called when the player selects the third save slot
    public void LoadGameDialog_Save3()
    {
        if (PlayerPrefs.HasKey("SavedLevel3")) // Check if a save exists in slot 3
        {
            PlayerPrefs.SetInt("LoadIndex", 3);
            SceneManager.LoadScene(firstGameSceneIndex);
        }
        else
        {
            noSavesFoundDialog.SetActive(true); // Show dialog if no save found
        }
    }
    public void DisableNoSaveFoundDialog()
    {
        noSavesFoundDialog.SetActive(false);
    }

    // Called when the player presses the "Exit" button
    public void ExitsButton()
    {
        Application.Quit(); // Closes the application
    }
}
