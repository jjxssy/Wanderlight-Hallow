using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages user-configurable key bindings for gameplay actions,
/// including persistence via <see cref="PlayerPrefs"/>, duplicate-key
/// detection, and UI row hookup for rebinding.
/// </summary>
public class KeyBindingsManager : MonoBehaviour
{
    /// <summary>
    /// UI rows that represent each bindable action in the settings screen.
    /// </summary>
    [SerializeField] private List<KeyBindingRow> keyBindingRows = new List<KeyBindingRow>();

    /// <summary>
    /// Optional popup GameObject shown when the player tries to assign
    /// a key that is already in use by another action.
    /// </summary>
    [SerializeField] private GameObject duplicateKeyPopup;

    /// <summary>
    /// Raised after bindings change (e.g., when a key is successfully rebound
    /// or when bindings are reset). Subscribers should refresh any cached input.
    /// </summary>
    public static event System.Action OnKeyBindingsChanged;

    /// <summary>
    /// The default mapping for all supported actions.
    /// Keys are action names in UPPER_CASE.
    /// </summary>
    private readonly Dictionary<string, KeyCode> defaultKeyMap = new Dictionary<string, KeyCode>()
    {
        { "WALK FORWARD", KeyCode.W },
        { "WALK BACKWARDS", KeyCode.S },
        { "WALK RIGHT", KeyCode.D },
        { "WALK LEFT", KeyCode.A },
        { "INVENTORY", KeyCode.I },
        { "SKILLS", KeyCode.K },
        { "INTERACTION", KeyCode.F },
        { "PAUSE MENU", KeyCode.Escape },
        { "SKILL 1", KeyCode.T },
        { "SKILL 2", KeyCode.Y },
        { "SKILL 3", KeyCode.U },
        { "SKILL 4", KeyCode.G },
        { "SKILL 5", KeyCode.H },
        { "SKILL 6", KeyCode.J },
        { "QUICKSLOT 1", KeyCode.Alpha1 },
        { "QUICKSLOT 2", KeyCode.Alpha2 },
        { "QUICKSLOT 3", KeyCode.Alpha3 },
        { "QUICKSLOT 4", KeyCode.Alpha4 },
        { "QUICKSLOT 5", KeyCode.Alpha5 }
    };

    /// <summary>
    /// The live, effective bindings used by gameplay systems.
    /// </summary>
    private readonly Dictionary<string, KeyCode> currentBindings = new Dictionary<string, KeyCode>();

    /// <summary>
    /// The user’s custom bindings as last set and saved.
    /// </summary>
    private readonly Dictionary<string, KeyCode> customBindings = new Dictionary<string, KeyCode>();

    /// <summary>
    /// The UI row currently waiting for a key press to complete a rebind, or <c>null</c>.
    /// </summary>
    private KeyBindingRow waitingForRow;

    /// <summary>
    /// The action name (UPPER_CASE) that is currently being rebound, or empty if none.
    /// </summary>
    private string waitingForAction;

    /// <summary>
    /// Loads bindings from <see cref="PlayerPrefs"/>, initializes UI rows,
    /// and ensures the duplicate popup is hidden initially.
    /// </summary>
    private void Start()
    {
        LoadBindings();
        SetupRows();

        if (duplicateKeyPopup != null)
        {
            duplicateKeyPopup.SetActive(false);
        }
    }

    /// <summary>
    /// While a rebind is in progress, listens for any key press,
    /// validates duplicates, and commits the new binding.
    /// </summary>
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
                        currentBindings[waitingForAction] = key;
                        customBindings[waitingForAction] = key;

                        PlayerPrefs.SetString(waitingForAction, key.ToString());
                        PlayerPrefs.Save();

                        waitingForRow.UpdateKey(key);

                        OnKeyBindingsChanged?.Invoke();
                    }
                    else
                    {
                        Debug.LogWarning($"Key {key} already in use!");
                        if (duplicateKeyPopup != null)
                        {
                            duplicateKeyPopup.SetActive(true);
                        }
                    }

                    waitingForRow = null;
                    waitingForAction = string.Empty;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Loads saved bindings from <see cref="PlayerPrefs"/> or falls back to defaults.
    /// Populates both <see cref="currentBindings"/> and <see cref="customBindings"/>.
    /// </summary>
    private void LoadBindings()
    {
        currentBindings.Clear();
        customBindings.Clear();

        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name.ToUpper();
            string saved = PlayerPrefs.GetString(actionName, GetDefaultKey(actionName).ToString());
            KeyCode savedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), saved);

            currentBindings[actionName] = savedKey;
            customBindings[actionName] = savedKey;
        }
    }

    /// <summary>
    /// Pushes the currently effective key for each action to its corresponding UI row.
    /// </summary>
    private void SetupRows()
    {
        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name.ToUpper();
            if (currentBindings.TryGetValue(actionName, out KeyCode key))
            {
                row.Setup(this, actionName, key);
            }
        }
    }

    /// <summary>
    /// Called by a <see cref="KeyBindingRow"/> when the player clicks its “rebind” button.
    /// Puts the manager into a “waiting for key” state for that action.
    /// </summary>
    /// <param name="row">The UI row initiating the change.</param>
    /// <param name="actionName">The action name to rebind (case-insensitive; will be uppercased).</param>
    public void RequestKeyChange(KeyBindingRow row, string actionName)
    {
        waitingForRow = row;
        waitingForAction = actionName.ToUpper();
    }

    /// <summary>
    /// Returns whether a given key is already bound to another action.
    /// </summary>
    /// <param name="key">Key to check.</param>
    /// <returns><c>true</c> if the key is already in use; otherwise <c>false</c>.</returns>
    private bool IsKeyAlreadyUsed(KeyCode key)
    {
        foreach (var binding in currentBindings.Values)
        {
            if (binding == key)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Restores all actions to their default keys, saves to <see cref="PlayerPrefs"/>,
    /// refreshes UI rows, and raises <see cref="OnKeyBindingsChanged"/>.
    /// </summary>
    public void ResetBindings()
    {
        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name.ToUpper();
            KeyCode defaultKey = GetDefaultKey(actionName);

            currentBindings[actionName] = defaultKey;
            customBindings[actionName] = defaultKey;
            PlayerPrefs.SetString(actionName, defaultKey.ToString());
        }

        PlayerPrefs.Save();
        SetupRows();

        OnKeyBindingsChanged?.Invoke();
    }

    /// <summary>
    /// Hides the duplicate-key popup if one was shown.
    /// </summary>
    public void ClosePopup()
    {
        if (duplicateKeyPopup != null)
        {
            duplicateKeyPopup.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the currently effective key bound to the given action.
    /// </summary>
    /// <param name="actionName">Action name (case-insensitive; will be uppercased internally).</param>
    /// <returns>The bound <see cref="KeyCode"/>; returns <see cref="KeyCode.None"/> if not found.</returns>
    public KeyCode GetKey(string actionName)
    {
        actionName = actionName.ToUpper();
        if (currentBindings.TryGetValue(actionName, out KeyCode key))
        {
            return key;
        }
        return KeyCode.None;
    }

    /// <summary>
    /// Returns the default key for an action, or <see cref="KeyCode.None"/> if the action is unknown.
    /// </summary>
    /// <param name="actionName">Action name (UPPER_CASE).</param>
    /// <returns>The default <see cref="KeyCode"/> for the action.</returns>
    private KeyCode GetDefaultKey(string actionName)
    {
        if (defaultKeyMap.TryGetValue(actionName, out KeyCode key))
        {
            return key;
        }
        return KeyCode.None;
    }
}
