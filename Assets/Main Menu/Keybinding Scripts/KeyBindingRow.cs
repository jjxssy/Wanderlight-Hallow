using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Represents a single row in the key bindings UI for one specific action.
/// Each row shows the action name, the currently bound key, and allows rebinding via a button.
/// </summary>
public class KeyBindingRow : MonoBehaviour
{
    /// <summary>
    /// UI element displaying the name of the action (e.g., "JUMP", "FIRE").
    /// </summary>
    [SerializeField] private TextMeshProUGUI actionNameText;

    /// <summary>
    /// UI element displaying the currently assigned key.
    /// </summary>
    [SerializeField] private TextMeshProUGUI keyText;

    /// <summary>
    /// Button used to initiate key change for this action.
    /// </summary>
    [SerializeField] private Button changeKeyButton;

    /// <summary>
    /// Reference to the <see cref="KeyBindingsManager"/> to communicate changes.
    /// </summary>
    private KeyBindingsManager manager;

    /// <summary>
    /// The name of the action this row represents.
    /// </summary>
    private string actionName;

    /// <summary>
    /// Initializes the row with action data and its current key binding.
    /// </summary>
    /// <param name="manager">The <see cref="KeyBindingsManager"/> handling bindings.</param>
    /// <param name="actionName">The name of the action this row represents.</param>
    /// <param name="defaultKey">The key currently bound to this action.</param>
    public void Setup(KeyBindingsManager manager, string actionName, KeyCode defaultKey)
    {
        this.manager = manager;
        this.actionName = actionName;

        // Set the UI text elements.
        actionNameText.text = actionName;
        keyText.text = defaultKey.ToString();

        // Add listener to the change key button.
        changeKeyButton.onClick.AddListener(OnChangeKeyClicked);
    }

    /// <summary>
    /// Updates the displayed key when a new key is bound to this action.
    /// </summary>
    /// <param name="newKey">The new <see cref="KeyCode"/> assigned to the action.</param>
    public void UpdateKey(KeyCode newKey)
    {
        keyText.text = newKey.ToString();
    }

    /// <summary>
    /// Handles the key change request when the "Change Key" button is clicked.
    /// </summary>
    private void OnChangeKeyClicked()
    {
        manager.RequestKeyChange(this, actionName);
    }
}
