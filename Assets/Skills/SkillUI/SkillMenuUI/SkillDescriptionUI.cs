using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the skill description panel: shows name, description, stat details,
/// cooldown, and an optional stat icon when a skill is hovered/selected.
/// </summary>
public class SkillDescriptionUI : MonoBehaviour
{
    /// <summary>Root panel that contains the description UI.</summary>
    [SerializeField] private GameObject descriptionPanel;

    /// <summary>Text label for the skill's display name.</summary>
    [SerializeField] private TextMeshProUGUI skillNameText;

    /// <summary>Text label for the skill's description/body.</summary>
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    /// <summary>Text label for stat-related details (e.g., buff value/type).</summary>
    [SerializeField] private TextMeshProUGUI skillStatText;

    /// <summary>Text label for the skill cooldown in seconds.</summary>
    [SerializeField] private TextMeshProUGUI skillCooldownText;

    /// <summary>Optional icon representing the stat/type of the skill.</summary>
    [SerializeField] private Image statIcon;

    /// <summary>
    /// Hides the description panel on startup.
    /// </summary>
    private void Start()
    {
        ClearDescription();
    }

    /// <summary>
    /// Populates and shows the description panel for the given skill.
    /// Pass <c>null</c> to clear/hide the panel.
    /// </summary>
    /// <param name="skill">Skill to display, or <c>null</c> to clear.</param>
    public void DisplaySkill(Skill skill)
    {
        if (skill == null)
        {
            ClearDescription();
            return;
        }

        descriptionPanel.SetActive(true);
        skillNameText.text        = skill.GetSkillName();
        skillDescriptionText.text = skill.GetDescription();
        skillStatText.text        = skill.GetStatDetails();
        skillCooldownText.text    = $"Cooldown: {skill.GetCooldown()}s";

        var icon = skill.GetStatIcon();
        if (icon != null)
        {
            statIcon.sprite  = icon;
            statIcon.enabled = true;
        }
        else
        {
            statIcon.enabled = false;
        }
    }

    /// <summary>
    /// Hides the description panel and clears any displayed info.
    /// </summary>
    public void ClearDescription()
    {
        descriptionPanel.SetActive(false);
    }
}
