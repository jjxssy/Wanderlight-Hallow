using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TextMeshProUGUI keybindText;

    private Skill assignedSkill;
    private float cooldownEndTime;

    private void Start()
    {
        // Ensure the overlay is hidden initially
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0;
        }
    }

    private void Update()
    {
        // Handle the cooldown visual
        if (assignedSkill != null && cooldownEndTime > Time.time)
        {
            cooldownOverlay.fillAmount = (cooldownEndTime - Time.time) / assignedSkill.cooldown;
        }
        else if (cooldownOverlay.fillAmount > 0)
        {
            cooldownOverlay.fillAmount = 0;
        }
    }
    public void SetSkill(Skill skill, KeyCode key)
    {
        assignedSkill = skill;

        if (assignedSkill != null)
        {
            skillIcon.sprite = assignedSkill.skillIcon;
            skillIcon.enabled = true;
            keybindText.text = key.ToString();
        }
        else
        {
            // If no skill is assigned, hide the icon
            skillIcon.sprite = null;
            skillIcon.enabled = false;
            keybindText.text = "";
        }
    }

    public void StartCooldown()
    {
        if (assignedSkill != null)
        {
            cooldownEndTime = Time.time + assignedSkill.cooldown;
        }
    }
}