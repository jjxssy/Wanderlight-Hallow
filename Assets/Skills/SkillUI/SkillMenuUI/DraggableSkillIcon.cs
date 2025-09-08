using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Draggable UI icon for a skill: supports drag & drop, hover description,
/// and restores itself when dropped outside valid targets.
/// </summary>
public class DraggableSkillIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>The skill represented by this icon.</summary>
    private Skill skill;

    /// <summary>UI image component showing the skill's icon.</summary>
    private Image iconImage;

    /// <summary>Parent transform the icon returns to after dragging ends.</summary>
    private Transform originalParent;

    /// <summary>Root canvas used so the icon can render above other UI while dragging.</summary>
    private Canvas rootCanvas;

    /// <summary>Panel that displays the skill's description on hover.</summary>
    private SkillDescriptionUI skillDescriptionPanel;

    /// <summary>
    /// Initializes the icon visuals and references for dragging and descriptions.
    /// </summary>
    /// <param name="skillToSet">The skill to associate with this icon.</param>
    /// <param name="descriptionPanel">The panel used to show the skill's details.</param>
    public void Initialize(Skill skillToSet, SkillDescriptionUI descriptionPanel)
    {
        iconImage = GetComponent<Image>();
        rootCanvas = GetComponentInParent<Canvas>();

        this.skill = skillToSet;
        this.skillDescriptionPanel = descriptionPanel;

        // Use getters (private fields in Skill)
        if (iconImage != null && skillToSet != null)
        {
            iconImage.sprite = skillToSet.GetSkillIcon();
        }
    }

    /// <summary>
    /// Returns the skill associated with this icon.
    /// </summary>
    public Skill GetSkill()
    {
        return skill;
    }

    /// <summary>
    /// Begins dragging: disables raycast on the icon and moves it under the root canvas.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (iconImage == null || rootCanvas == null) return;

        // Make the icon see-through and move it to the top layer of the canvas so it's visible everywhere
        iconImage.raycastTarget = false;
        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform);
    }

    /// <summary>
    /// While dragging, the icon follows the pointer position.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    /// <summary>
    /// Ends dragging: re-enable raycast, restore parent, and reset local position.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (iconImage == null) return;

        // Return the icon to its original spot and make it clickable again
        iconImage.raycastTarget = true;
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// On hover enter, shows the skill description if a panel is assigned.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillDescriptionPanel != null)
        {
            skillDescriptionPanel.DisplaySkill(skill);
        }
    }

    /// <summary>
    /// On hover exit, clears the skill description.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillDescriptionPanel != null)
        {
            skillDescriptionPanel.ClearDescription();
        }
    }
}