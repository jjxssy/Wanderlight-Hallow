using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableSkillIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Skill skill;
    private Image iconImage;
    private Transform originalParent;
    private Canvas rootCanvas;

    private SkillDescriptionUI skillDescriptionPanel;

    public void Initialize(Skill skillToSet, SkillDescriptionUI descriptionPanel)
    {
        iconImage = GetComponent<Image>();
        rootCanvas = GetComponentInParent<Canvas>();

        this.skill = skillToSet;
        this.skillDescriptionPanel = descriptionPanel;

        iconImage.sprite = skillToSet.skillIcon;
    }

    public Skill GetSkill()
    {
        return skill;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Make the icon see-through and move it to the top layer of the canvas so it's visible everywhere
        iconImage.raycastTarget = false;
        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // The icon follows the mouse
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Return the icon to its original spot and make it clickable again
        iconImage.raycastTarget = true;
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero; 
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillDescriptionPanel != null)
        {
            skillDescriptionPanel.DisplaySkill(skill);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillDescriptionPanel != null)
        {
            skillDescriptionPanel.ClearDescription();
        }
    }
}