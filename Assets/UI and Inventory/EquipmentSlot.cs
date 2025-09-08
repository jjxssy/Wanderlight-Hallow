using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Represents a UI slot for equipping gear. 
/// Handles drops, clicks, and tooltip display for equipment items.
/// </summary>
public class EquipmentSlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Slot Configuration")]
    [Tooltip("The type of equipment this slot is designed to hold.")]
    [SerializeField] private EquipmentType expectedType;

    [Header("UI References")]
    [Tooltip("The child Image component that displays the item's icon.")]
    [SerializeField] private Image iconImage;
    
    /// <summary>
    /// The item currently held in this slot.
    /// </summary>
    public Item HeldItem { get; private set; }

    private Image backgroundImage;

    private void Awake()
    {
        if (iconImage == null)
        {
            Debug.LogError($"EquipmentSlot '{gameObject.name}' is missing its Icon Image reference!", this.gameObject);
        }

        backgroundImage = GetComponent<Image>();
        UpdateSlotUI();

        // ClearSlot();
    }

    /// <summary>
    /// Handles click events on the slot.
    /// Clicking an equipped item attempts to unequip it.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (HeldItem == null) return;

        // On any click, attempt to unequip the item.
        EquipmentManager.instance.UnequipItem(HeldItem);
    }


    /// <summary>
    /// Handles when an item is dropped onto this slot.
    /// Validates type before equipping.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (InventorySlot.DraggedSlot == null) return;

        Item draggedItem = InventorySlot.DraggedSlot.HeldItem;
        if (draggedItem != null && draggedItem.GetEquipmentType() == expectedType)
        {
            EquipmentManager.instance.EquipItem(draggedItem, InventorySlot.DraggedSlot.SlotIndex);
        }
    }

     /// <summary>
    /// Sets the item in this equipment slot and updates UI.
    /// </summary>
    public void SetItem(Item item)
    {
        HeldItem = item;
        UpdateSlotUI();
    }

    /// <summary>
    /// Clears this equipment slot.
    /// </summary>
    public void ClearSlot()
    {
        HeldItem = null;
        UpdateSlotUI();
    }
    
    /// <summary>
    /// Updates the UI to reflect current held item state.
    /// </summary>
    private void UpdateSlotUI()
    {
        if (iconImage == null) return;

        bool hasItem = (HeldItem != null);

        // Show/hide the item icon
        iconImage.sprite = hasItem ? HeldItem.GetIcon() : null;
        iconImage.enabled = hasItem;

        // --- NEW: Show/hide the background slot image ---
        if (backgroundImage != null)
        {
            backgroundImage.enabled = !hasItem;
        }
    }

    // --- Tooltip Handlers ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HeldItem != null)
        {
            string tooltipContent = $"<b>{HeldItem.GetItemName()}</b>\n<size=18>{HeldItem.GetDescription()}</size>";
            TooltipManager.GetInstance().ShowTooltip(tooltipContent);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.GetInstance().HideTooltip();
    }
}

