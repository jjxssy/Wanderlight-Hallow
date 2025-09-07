using UnityEngine;
/// <summary>
/// Handles UI interactions for the game.
/// Specifically manages toggling of the death panel.
/// </summary>

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the death panel GameObject.
    /// Assigned in the Inspector.
    /// </summary>
    [SerializeField] private GameObject deathPanel;

    /// <summary>
    /// Toggles the active state of the death panel.
    /// </summary>
    public void ToggleDeathPanel(){
        deathPanel.SetActive(!deathPanel.activeSelf);
    }
}
