using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Represents a single row in the key bindings UI for one specific action.
public class KeyBindingRow : MonoBehaviour
{
    // UI element displaying the name of the action (e.g., "JUMP", "FIRE").
    [SerializeField] private TextMeshProUGUI actionNameText;

    // UI element displaying the currently assigned key.
    [SerializeField] private TextMeshProUGUI keyText;

    // Button used to initiate key change for this action.
    [SerializeField] private Button changeKeyButton;

    // Reference to the KeyBindingsManager to communicate changes.
    private KeyBindingsManager manager;

    // Name of the action this row represents.
    private string actionName;

    // Initializes the row with action data and key binding.
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

    // Called to update the displayed key when a new key is bound.
    public void UpdateKey(KeyCode newKey)
    {
        keyText.text = newKey.ToString();
    }

    // Handles the key change request when button is clicked.
    private void OnChangeKeyClicked()
    {
        manager.RequestKeyChange(this, actionName);
    }
}
