using UnityEngine;

/// <summary>
/// Manages the game-over flow, toggling the death UI when needed.
/// </summary>
public class DeathManager : MonoBehaviour
{
    public static DeathManager instance;

    /// <summary>
    /// Ensures a single instance of DeathManager exists.
    /// </summary>
    private void Awake(){
        if(DeathManager.instance==null)
            instance=this;
            else 
                Destroy(gameObject);
    }

    /// <summary>
    /// Triggers the game-over UI by toggling the death panel on the attached UIManager.
    /// </summary>
    public void GameOver(){
        UIManager _ui= GetComponent<UIManager>();
        if(_ui!=null)
            _ui.ToggleDeathPanel();
    }
}
