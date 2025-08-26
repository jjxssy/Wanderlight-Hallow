using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDescriptionUI : MonoBehaviour
{
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private TextMeshProUGUI skillStatText;
    [SerializeField] private TextMeshProUGUI skillCooldownText;
    [SerializeField] private Image statIcon;

    void Start()
    {
        ClearDescription();
    }

    public void DisplaySkill(Skill skill)
    {
        if (skill == null)
        {
            ClearDescription();
            return;
        }

        descriptionPanel.SetActive(true);
        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;
        skillStatText.text = skill.GetStatDetails();
        skillCooldownText.text = $"Cooldown: {skill.cooldown}s";

        if (skill.statIcon != null)
        {
            statIcon.sprite = skill.statIcon;
            statIcon.enabled = true;
        }
        else
        {
            statIcon.enabled = false;
        }
    }

    public void ClearDescription()
    {
        descriptionPanel.SetActive(false);
    }
}