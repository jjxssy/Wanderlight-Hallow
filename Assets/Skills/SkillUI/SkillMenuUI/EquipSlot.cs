using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Skill-bar equip slot that accepts dragged skill icons, shows hover info,
/// and updates its icon when the equipped skills change.
/// </summary>
public class EquipSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Index of this slot in the player's equipped skills array (0-based).
    /// </summary>
    [SerializeField] private int slotIndex;

    /// <summary>Reference to the player's skill manager (resolved at runtime).</summary>
    private PlayerSkillManager playerSkillManager;

    /// <summary>Reference to the panel that displays skill descriptions.</summary>
    private SkillDescriptionUI skillDescriptionPanel;

    /// <summary>Image component used to render the equipped skill icon.</summary>
    private Image image;

    /// <summary>
    /// Grabs required references (manager, image, description panel).
    /// </summary>
    private void Awake()
    {
        playerSkillManager = FindFirstObjectByType<PlayerSkillManager>();
        image = GetComponent<Image>();
        skillDescriptionPanel = GetComponentInParent<SkillMenu>().GetDescriptionPannel();
    }

    /// <summary>
    /// Subscribes to equipped-skill change notifications and refreshes the visual.
    /// </summary>
    private void OnEnable()
    {
        if (playerSkillManager != null)
        {
            // event is private on PlayerSkillManager; use its listener methods
            playerSkillManager.AddOnEquippedSkillsChangedListener(UpdateSlotVisual);
        }
        UpdateSlotVisual();
    }

    /// <summary>
    /// Unsubscribes from notifications to avoid dangling listeners.
    /// </summary>
    private void OnDisable()
    {
        if (playerSkillManager != null)
        {
            playerSkillManager.RemoveOnEquippedSkillsChangedListener(UpdateSlotVisual);
        }
    }

    /// <summary>
    /// Handles a draggable skill icon being dropped on this slot and equips it.
    /// </summary>
    /// <param name="eventData">Pointer event data for the drop.</param>
    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        var droppedIcon = eventData.pointerDrag?.GetComponent<DraggableSkillIcon>();
        if (droppedIcon != null && playerSkillManager != null)
        {
            playerSkillManager.EquipSkill(slotIndex, droppedIcon.GetSkill());
            if (skillDescriptionPanel != null)
            {
                skillDescriptionPanel.DisplaySkill(droppedIcon.GetSkill());
            }
        }
    }

    /// <summary>
    /// On hover, shows the skill currently equipped in this slot (if any).
    /// </summary>
    /// <param name="eventData">Pointer event data for the enter.</param>
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (playerSkillManager != null && skillDescriptionPanel != null)
        {
            Skill skillInSlot = playerSkillManager.GetEquipedSkills()[slotIndex];
            if (skillInSlot != null)
            {
                skillDescriptionPanel.DisplaySkill(skillInSlot);
            }
        }
    }

    /// <summary>
    /// On hover exit, clears the description panel.
    /// </summary>
    /// <param name="eventData">Pointer event data for the exit.</param>
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (skillDescriptionPanel != null)
        {
            skillDescriptionPanel.ClearDescription();
        }
    }

    /// <summary>
    /// Syncs the slot's icon and visibility with the currently equipped skill.
    /// Uses getters because Skill now has private fields only.
    /// </summary>
    private void UpdateSlotVisual()
    {
        if (playerSkillManager == null) return;

        Skill equippedSkill = playerSkillManager.GetEquipedSkills()[slotIndex];
        if (equippedSkill != null)
        {
            image.sprite = equippedSkill.GetSkillIcon();
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
        }
    }
}
