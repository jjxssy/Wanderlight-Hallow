using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Manages the main menu functionality such as starting a new game, loading saves, and exiting the game
public class MainMenuManager : MonoBehaviour
{

    [Header("Save to Load")]
    public string _newGameLevel; // The scene name to load when starting a new game
    private string levelToLoad; // Internal variable to store the name of the level to load
    [SerializeField] private GameObject noSavesFoundDialog = null; // Dialog box to show when no saved games are found

    // Called when the player chooses "New Game" - loads the new game scene
    public void NewGameDialog()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    // Called when the player selects the first save slot
    public void LoadGameDialog_Save1()
    {
        if (PlayerPrefs.HasKey("SavedLevel1")) // Check if a save exists in slot 1
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel1"); // Get the saved level name
            SceneManager.LoadScene(levelToLoad); // Load the saved scene
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
            levelToLoad = PlayerPrefs.GetString("SavedLevel2"); // Get the saved level name
            SceneManager.LoadScene(levelToLoad); // Load the saved scene
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
            levelToLoad = PlayerPrefs.GetString("SavedLevel3"); // Get the saved level name
            SceneManager.LoadScene(levelToLoad); // Load the saved scene
        }
        else
        {
            noSavesFoundDialog.SetActive(true); // Show dialog if no save found
        }
    }

    // Called when the player presses the "Exit" button
    public void ExitsButton()
    {
        Application.Quit(); // Closes the application
    }
}
