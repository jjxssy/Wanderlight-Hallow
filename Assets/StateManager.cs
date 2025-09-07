using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene transitions such as reloading the current scene
/// or switching to another scene by name.
/// </summary>
public class StateManager : MonoBehaviour
{
    /// <summary>
    /// Reloads the currently active scene by build index.
    /// </summary>
    public void ReloadCurrentScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    /// <summary>
    /// Loads a new scene by its name.
    /// </summary>
    /// <param name="name">The name of the scene to load.</param>
    public void ChangeSceneByName(string name){
        if(name!=null)
            SceneManager.LoadScene(name);
    }

}
