using UnityEngine;

/// <summary>
/// Central UI controller for in-game panels and overlays.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Panel shown on player death.
    /// </summary>
    [SerializeField] private GameObject deathPanel;

    /// <summary>
    /// Toggles the visibility of the death panel.
    /// </summary>
    public void ToggleDeathPanel()
    {
        deathPanel.SetActive(!deathPanel.activeSelf);
    }
}
