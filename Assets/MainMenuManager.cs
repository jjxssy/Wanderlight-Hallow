using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour

{
    [Header("Save to Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavesFoundDialog = null;

    public void NewGameDialog()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialog_Save1()
    {
        if (PlayerPrefs.HasKey("SavedLevel1"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel1");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavesFoundDialog.SetActive(true);
        }

    }

    public void LoadGameDialog_Save2()
    {
        if (PlayerPrefs.HasKey("SavedLevel2"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel2");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavesFoundDialog.SetActive(true);
        }
    }

    public void LoadGameDialog_Save3()
    {
        if (PlayerPrefs.HasKey("SavedLevel3"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel3");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavesFoundDialog.SetActive(true);
        }
    }
public void ExitsButton()
    {
        Application.Quit();
    }
    
}
