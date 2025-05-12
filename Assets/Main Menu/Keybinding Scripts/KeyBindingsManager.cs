using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script manages custom key bindings for game actions.
public class KeyBindingsManager : MonoBehaviour
{
    // List of UI rows representing each action and its key binding.
    [SerializeField] private List<KeyBindingRow> keyBindingRows = new List<KeyBindingRow>();
    
    // Button used to reset all bindings to default.
    [SerializeField] private Button resetButton;

    // Dictionary storing current key bindings (action name -> KeyCode).
    private Dictionary<string, KeyCode> currentBindings = new Dictionary<string, KeyCode>();

    // Variables for handling a pending key binding change.
    private KeyBindingRow waitingForRow;
    private string waitingForAction;

    // Called when the script instance is loaded.
    private void Start()
    {
        // Add listener to reset button.
        resetButton.onClick.AddListener(ResetBindings);

        // Load saved or default key bindings.
        LoadBindings();

        // Populate UI rows with current key bindings.
        SetupRows();
    }

    // Load key bindings from PlayerPrefs or use default (KeyCode.None).
    private void LoadBindings()
    {
        currentBindings.Clear();

        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name; // Could be customized
            KeyCode savedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(actionName, KeyCode.None.ToString()));
            currentBindings[actionName] = savedKey;
        }
    }

    // Assigns key bindings to the corresponding UI rows.
    private void SetupRows()
    {
        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name; // Could be changed to a custom field
            if (currentBindings.TryGetValue(actionName, out KeyCode key))
            {
                row.Setup(this, actionName, key);
            }
        }
    }

    // Called when a user clicks to change a key binding.
    public void RequestKeyChange(KeyBindingRow row, string actionName)
    {
        waitingForRow = row;
        waitingForAction = actionName;
    }

    // Checks if the user has pressed a key while waiting for input, then updates the binding.
    private void Update()
    {
        if (waitingForRow != null)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (!IsKeyAlreadyUsed(key))
                    {
                        // Save new key binding.
                        currentBindings[waitingForAction] = key;
                        PlayerPrefs.SetString(waitingForAction, key.ToString());
                        PlayerPrefs.Save();

                        // Update the UI.
                        waitingForRow.UpdateKey(key);
                    }
                    else
                    {
                        Debug.LogWarning($"Key {key} already in use!");
                    }

                    // Clear waiting state.
                    waitingForRow = null;
                    waitingForAction = "";
                    break;
                }
            }
        }
    }

    // Checks if a key is already assigned to another action.
    private bool IsKeyAlreadyUsed(KeyCode key)
    {
        foreach (var binding in currentBindings.Values)
        {
            if (binding == key)
                return true;
        }
        return false;
    }

    // Resets all key bindings to default (currently KeyCode.None).
    private void ResetBindings()
    {
        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name;
            KeyCode defaultKey = KeyCode.None; // Replace with actual defaults if needed

            currentBindings[actionName] = defaultKey;
            PlayerPrefs.SetString(actionName, defaultKey.ToString());
        }

        PlayerPrefs.Save();

        // Update UI to reflect default bindings.
        SetupRows();
    }
}
