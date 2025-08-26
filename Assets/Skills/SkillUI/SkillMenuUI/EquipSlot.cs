using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;

    private PlayerSkillManager playerSkillManager;
    private SkillDescriptionUI skillDescriptionPanel;
    private Image image;

    void Awake()
    {
        playerSkillManager = FindFirstObjectByType<PlayerSkillManager>();
        image = GetComponent<Image>();

        skillDescriptionPanel = GetComponentInParent<SkillMenu>().GetDescriptionPannel();
    }

    private void OnEnable()
    {
        // Subscribe to the event when the object is enabled
        if (playerSkillManager != null)
        {
            playerSkillManager.OnEquippedSkillsChanged += UpdateSlotVisual;
        }
        // Immediately update the visual when the menu is opened
        UpdateSlotVisual();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent errors when the object is disabled
        if (playerSkillManager != null)
        {
            playerSkillManager.OnEquippedSkillsChanged -= UpdateSlotVisual;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableSkillIcon droppedIcon = eventData.pointerDrag.GetComponent<DraggableSkillIcon>();

        if (droppedIcon != null && playerSkillManager != null)
        {
            // Just tell the manager to equip the skill.
            // The event system will handle updating the visual.
            playerSkillManager.EquipSkill(slotIndex, droppedIcon.GetSkill());

            skillDescriptionPanel.DisplaySkill(droppedIcon.GetSkill());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerSkillManager != null && skillDescriptionPanel != null)
        {
            // Get the skill currently in this slot
            Skill skillInSlot = playerSkillManager.GetEquipedSkills()[slotIndex];
            if (skillInSlot != null)
            {
                skillDescriptionPanel.DisplaySkill(skillInSlot);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillDescriptionPanel != null)
        {
            skillDescriptionPanel.ClearDescription();
        }
    }
    private void UpdateSlotVisual()
    {
        if (playerSkillManager == null) return;

        // Get the skill that should be in this slot
        Skill equippedSkill = playerSkillManager.GetEquipedSkills()[slotIndex];
        if (equippedSkill != null)
        {
            image.sprite = equippedSkill.skillIcon;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
        }
    }
}