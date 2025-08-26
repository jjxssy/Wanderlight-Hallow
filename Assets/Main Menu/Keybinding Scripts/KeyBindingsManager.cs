using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingsManager : MonoBehaviour
{
    [SerializeField] private List<KeyBindingRow> keyBindingRows = new List<KeyBindingRow>();
    [SerializeField] private GameObject duplicateKeyPopup; // 👈 Reference to the popup

    public static event System.Action OnKeyBindingsChanged;

    private Dictionary<string, KeyCode> defaultKeyMap = new Dictionary<string, KeyCode>()
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

    private Dictionary<string, KeyCode> currentBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> customBindings = new Dictionary<string, KeyCode>();

    private KeyBindingRow waitingForRow;
    private string waitingForAction;

    private void Start()
    {
        LoadBindings();
        SetupRows();

        if (duplicateKeyPopup != null)
        {
            duplicateKeyPopup.SetActive(false); // Make sure it starts hidden
        }
    }

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

    public void RequestKeyChange(KeyBindingRow row, string actionName)
    {
        waitingForRow = row;
        waitingForAction = actionName.ToUpper();
    }

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
                    waitingForAction = "";
                    break;
                }
            }
        }
    }

    private bool IsKeyAlreadyUsed(KeyCode key)
    {
        foreach (var binding in currentBindings.Values)
        {
            if (binding == key)
                return true;
        }
        return false;
    }

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


    private KeyCode GetDefaultKey(string actionName)
    {
        if (defaultKeyMap.TryGetValue(actionName, out KeyCode key))
        {
            return key;
        }
        return KeyCode.None;
    }

    public void ClosePopup()
    {
        if (duplicateKeyPopup != null)
        {
            duplicateKeyPopup.SetActive(false);
        }
    }

    public KeyCode GetKey(string actionName)
    {
        actionName = actionName.ToUpper();
        if (currentBindings.TryGetValue(actionName, out KeyCode key))
        {
            return key;

        }
        return KeyCode.None;
    }

}
