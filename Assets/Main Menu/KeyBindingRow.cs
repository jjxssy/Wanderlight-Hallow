using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyBindingRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private Button changeKeyButton;

    private KeyBindingsManager manager;
    private string actionName;

    public void Setup(KeyBindingsManager manager, string actionName, KeyCode defaultKey)
    {
        this.manager = manager;
        this.actionName = actionName;
        actionNameText.text = actionName;
        keyText.text = defaultKey.ToString();

        changeKeyButton.onClick.AddListener(OnChangeKeyClicked);
    }

    public void UpdateKey(KeyCode newKey)
    {
        keyText.text = newKey.ToString();
    }

    private void OnChangeKeyClicked()
    {
        manager.RequestKeyChange(this, actionName);
    }
}
