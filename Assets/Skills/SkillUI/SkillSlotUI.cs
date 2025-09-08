using UnityEngine;
using UnityEngine.UI;
using TMPro; 

/// <summary>
/// UI widget for a single skill slot: shows icon, bound key, and a radial cooldown overlay.
/// </summary>
public class SkillSlotUI : MonoBehaviour
{
    /// <summary>Image used to render the skill's icon.</summary>
    [SerializeField] private Image skillIcon;

    /// <summary>Radial image overlay that displays cooldown progress (fillAmount 0..1).</summary>
    [SerializeField] private Image cooldownOverlay;

    /// <summary>Label that shows the current key binding for this slot.</summary>
    [SerializeField] private TextMeshProUGUI keybindText;

    /// <summary>The skill currently assigned to this slot (null if empty).</summary>
    private Skill assignedSkill;

    /// <summary>World time at which the cooldown for the assigned skill ends.</summary>
    private float cooldownEndTime;

    /// <summary>
    /// Initializes visuals; ensures cooldown overlay starts hidden.
    /// </summary>
    private void Start()
    {
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0;
        }
    }

    /// <summary>
    /// Updates the cooldown overlay each frame while the skill is cooling down.
    /// </summary>
    private void Update()
    {
        if (assignedSkill != null && cooldownEndTime > Time.time)
        {
            cooldownOverlay.fillAmount = (cooldownEndTime - Time.time) / assignedSkill.GetCooldown();
        }
        else if (cooldownOverlay.fillAmount > 0)
        {
            cooldownOverlay.fillAmount = 0;
        }
    }

    /// <summary>
    /// Assigns a skill to this slot and updates the icon and key label.
    /// Pass null to clear the slot.
    /// </summary>
    public void SetSkill(Skill skill, KeyCode key)
    {
        assignedSkill = skill;

        if (assignedSkill != null)
        {
            skillIcon.sprite = assignedSkill.GetSkillIcon();
            skillIcon.enabled = true;
            keybindText.text = key.ToString();
        }
        else
        {
            skillIcon.sprite = null;
            skillIcon.enabled = false;
            keybindText.text = "";
        }
    }

    /// <summary>
    /// Starts the cooldown display based on the assigned skill's cooldown duration.
    /// </summary>
    public void StartCooldown()
    {
        if (assignedSkill != null)
        {
            cooldownEndTime = Time.time + assignedSkill.GetCooldown();
        }
    }
}
