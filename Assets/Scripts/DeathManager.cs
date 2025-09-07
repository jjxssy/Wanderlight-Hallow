using UnityEngine;
/// <summary>
/// Manages player death state and handles game-over behavior.
/// Implemented as a singleton to ensure only one instance exists.
/// </summary>
public class DeathManager : MonoBehaviour
{
    private static DeathManager instance;


    /// <summary>
    /// Ensures only one DeathManager exists in the scene.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Triggers the game-over sequence by toggling the death panel
    /// through the attached UIManager (if present).
    /// Call this from other scripts via <c>DeathManager.GameOver()</c>.
    /// </summary>
    public static void GameOver()
    {
        if (instance == null)
        {
            Debug.LogError("DeathManager.GameOver called, but no instance exists!");
            return;
        }
        UIManager ui = instance.GetComponent<UIManager>();
        if (ui != null)
        {
            ui.ToggleDeathPanel();
        }
    }
}
