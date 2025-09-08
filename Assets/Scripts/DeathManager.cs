using UnityEngine;

/// <summary>
/// Manages the game-over flow by toggling the death UI when the player dies.
/// Keep this on the same GameObject as a <see cref="UIManager"/>.
/// </summary>
public class DeathManager : MonoBehaviour
{
    // Private singleton field
    private static DeathManager instance;

    // Public Java-style getter for the singleton
    public static DeathManager GetInstance() { return instance; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Triggers the game-over UI by toggling the death panel on the attached <see cref="UIManager"/>.
    /// </summary>
    public void GameOver()
    {
        UIManager ui = GetComponent<UIManager>();
        if (ui != null)
            ui.ToggleDeathPanel();
    }
}
