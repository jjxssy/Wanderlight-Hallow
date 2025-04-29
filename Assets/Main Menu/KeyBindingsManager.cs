using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindingsManager : MonoBehaviour
{
    [SerializeField] private List<KeyBindingRow> keyBindingRows = new List<KeyBindingRow>();
    [SerializeField] private Button resetButton;

    private Dictionary<string, KeyCode> currentBindings = new Dictionary<string, KeyCode>();
    private KeyBindingRow waitingForRow;
    private string waitingForAction;

    private void Start()
    {
        resetButton.onClick.AddListener(ResetBindings);

        LoadBindings();
        SetupRows();
    }

    private void LoadBindings()
    {
        currentBindings.Clear();

        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name; // Or custom if you want
            KeyCode savedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(actionName, KeyCode.None.ToString()));
            currentBindings[actionName] = savedKey;
        }
    }

    private void SetupRows()
    {
        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name; // or custom field if you prefer
            if (currentBindings.TryGetValue(actionName, out KeyCode key))
            {
                row.Setup(this, actionName, key);
            }
        }
    }

    public void RequestKeyChange(KeyBindingRow row, string actionName)
    {
        waitingForRow = row;
        waitingForAction = actionName;
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
                        PlayerPrefs.SetString(waitingForAction, key.ToString());
                        PlayerPrefs.Save();

                        waitingForRow.UpdateKey(key);
                    }
                    else
                    {
                        Debug.LogWarning($"Key {key} already in use!");
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

    private void ResetBindings()
    {
        foreach (KeyBindingRow row in keyBindingRows)
        {
            string actionName = row.name; // Or custom
            KeyCode defaultKey = KeyCode.None; // You can define default somewhere smarter

            currentBindings[actionName] = defaultKey;
            PlayerPrefs.SetString(actionName, defaultKey.ToString());
        }

        PlayerPrefs.Save();
        SetupRows();
    }
}
