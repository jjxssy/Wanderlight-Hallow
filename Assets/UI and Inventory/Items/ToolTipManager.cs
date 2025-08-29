using UnityEngine;
using TMPro; // Make sure to import TextMeshPro

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform tooltipRect; // The RectTransform of the panel

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

        // Start with the tooltip hidden
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Make the tooltip follow the mouse cursor
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            // We add a small offset so the tooltip doesn't sit directly under the cursor
            tooltipRect.position = Input.mousePosition + new Vector3(10, -10, 0);
        }
    }

    public void ShowTooltip(string content)
    {
        if (tooltipPanel == null || tooltipText == null) return;

        tooltipText.text = content;
        tooltipPanel.SetActive(true);
    }
    public void HideTooltip()
    {
        if (tooltipPanel == null) return;

        tooltipPanel.SetActive(false);
        tooltipText.text = string.Empty;
    }
}
