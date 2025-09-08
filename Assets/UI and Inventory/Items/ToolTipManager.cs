using UnityEngine;
using TMPro; // Make sure to import TextMeshPro
/// <summary>
/// Manages showing and hiding tooltips in the UI.
/// Ensures only one instance exists (singleton).
/// </summary>
public class TooltipManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance for accessing the tooltip system.
    /// </summary>
    public static TooltipManager instance;

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform tooltipRect; // The RectTransform of the panel

    /// <summary>
    /// Ensures singleton instance and hides tooltip on startup.
    /// </summary>
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

    /// <summary>
    /// Updates tooltip position to follow the mouse cursor.
    /// </summary>
    private void Update()
    {
        // Make the tooltip follow the mouse cursor
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            // We add a small offset so the tooltip doesn't sit directly under the cursor
            tooltipRect.position = Input.mousePosition + new Vector3(10, -10, 0);
        }
    }

    /// <summary>
    /// Updates tooltip position to follow the mouse cursor.
    /// </summary>
    public void ShowTooltip(string content)
    {
        if (tooltipPanel == null || tooltipText == null) return;

        tooltipText.text = content;
        tooltipPanel.SetActive(true);
    }

    /// <summary>
    /// Hides the tooltip and clears its text.
    /// </summary>
    public void HideTooltip()
    {
        if (tooltipPanel == null) return;

        tooltipPanel.SetActive(false);
        tooltipText.text = string.Empty;
    }
}
