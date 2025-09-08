using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a single inventory slot in the UI:
/// - Stores a reference to the held <see cref="Item"/>
/// - Updates the slot icon
/// - Supports drag & drop operations
/// - Handles tooltip display
/// </summary>
public class InventorySlot : MonoBehaviour, IPointerClickHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [Tooltip("The Image component that displays the item's icon. This should be a child of the slot's background.")]
    [SerializeField] private Image iconImage;
    [Tooltip("A simple prefab with just an Image component to be used as a drag visual.")]
    [SerializeField] private GameObject dragIconPrefab;

    /// <summary>
    /// The item currently held in this slot (null if empty).
    /// </summary>
    public Item HeldItem { get; private set; }

    /// <summary>
    /// Index of this slot in the overall <see cref="InventoryManager"/>.
    /// </summary>
    public int SlotIndex { get; private set; }
    
    /// <summary>
    /// Temporary GameObject created when dragging the item icon.
    /// </summary>
    private GameObject _dragIconInstance;

    /// <summary>
    /// The slot currently being dragged (shared globally).
    /// </summary>
    private static InventorySlot _draggedSlot; 

    /// <summary>
    /// Public read-only access to the slot being dragged.
    /// </summary>
    public static InventorySlot DraggedSlot => _draggedSlot;

    /// <summary>
    /// Initializes this slot with a given index.
    /// </summary>
    public void Initialize(int index)
    {
        SlotIndex = index;
        HeldItem = null;
        UpdateSlotUI();
    }

    /// <summary>
    /// Places an item into this slot and updates its UI.
    /// </summary>
    public void SetItem(Item item)
    {
        HeldItem = item;
        UpdateSlotUI();
    }

    /// <summary>
    /// Clears this slot (removes the held item).
    /// </summary>
    public void ClearSlot()
    {
        HeldItem = null;
        UpdateSlotUI();
    }

    /// <summary>
    /// Clears this slot (removes the held item).
    /// </summary>
    private void UpdateSlotUI()
    {
        if (iconImage == null) return;

        if (HeldItem != null)
        {
            iconImage.sprite = HeldItem.GetIcon();
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    /// <summary>
    /// Handles right-click to use the held item.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (HeldItem == null) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.Instance.UseItem(SlotIndex);
        }
    }

    /// <summary>
    /// Handles dropping an item onto this slot from another.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (DraggedSlot != null)
        {
            InventoryManager.Instance.SwapItems(DraggedSlot.SlotIndex, this.SlotIndex);
        }
    }


    /// <summary>
    /// Called when dragging begins. Creates a drag icon and hides the original icon.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (HeldItem == null) return; // Can't drag an empty slot.

        _draggedSlot = this;

        // Create a visual representation of the item to drag.
        _dragIconInstance = Instantiate(dragIconPrefab, transform.root); // Instantiate on the Canvas
        _dragIconInstance.GetComponent<Image>().sprite = HeldItem.GetIcon();
        _dragIconInstance.transform.position = eventData.position;

        // ** FIX: Add a CanvasGroup to the drag icon to make it ignore raycasts. **
        // This allows the OnDrop event to be triggered by the slot underneath.
        CanvasGroup canvasGroup = _dragIconInstance.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = _dragIconInstance.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false;

        // Hide the original item icon while dragging.
        iconImage.enabled = false;
    }

    /// <summary>
    /// Called while dragging. Moves the drag icon with the cursor.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIconInstance != null)
        {
            _dragIconInstance.transform.position = eventData.position;
        }
    }

    /// <summary>
    /// Called when dragging ends. Cleans up drag icon and restores slot UI.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Clean up the created drag icon.
        if (_dragIconInstance != null)
        {
            Destroy(_dragIconInstance);
        }

        // Reset the static reference.
        _draggedSlot = null;

        // Always show the icon again, whether the swap was successful or not.
        UpdateSlotUI();
    }
    
    /// <summary>
    /// Shows the item tooltip when hovering over the slot.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HeldItem != null)
        {
            // Build a formatted string for the tooltip using the item's data
            string tooltipContent = $"<b>{HeldItem.GetItemName()}</b>\n<size=18>{HeldItem.GetDescription()}</size>";
            TooltipManager.GetInstance().ShowTooltip(tooltipContent);
        }
    }
    
    /// <summary>
    /// Hides the tooltip when the cursor exits the slot.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.GetInstance().HideTooltip();
    }
}
