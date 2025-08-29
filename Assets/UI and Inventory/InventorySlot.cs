using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [Tooltip("The Image component that displays the item's icon. This should be a child of the slot's background.")]
    [SerializeField] private Image iconImage;
    [Tooltip("A simple prefab with just an Image component to be used as a drag visual.")]
    [SerializeField] private GameObject dragIconPrefab;

    public Item HeldItem { get; private set; }
    public int SlotIndex { get; private set; }

    private GameObject _dragIconInstance;
    private static InventorySlot _draggedSlot; // Static reference to the slot being dragged

    public static InventorySlot DraggedSlot => _draggedSlot;

    public void Initialize(int index)
    {
        SlotIndex = index;
        HeldItem = null;
        UpdateSlotUI();
    }

    public void SetItem(Item item)
    {
        HeldItem = item;
        UpdateSlotUI();
    }

    public void ClearSlot()
    {
        HeldItem = null;
        UpdateSlotUI();
    }

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

    #region Event Handlers

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HeldItem == null) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.instance.UseItem(SlotIndex);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Check if an item was being dragged from another valid slot.
        if (_draggedSlot != null && _draggedSlot != this)
        {
            // Tell the manager to swap the items logically.
            InventoryManager.instance.SwapItems(_draggedSlot.SlotIndex, this.SlotIndex);
        }
    }

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


    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIconInstance != null)
        {
            _dragIconInstance.transform.position = eventData.position;
        }
    }

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

    #endregion
    #region Tooltip Handlers

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HeldItem != null)
        {
            // Build a formatted string for the tooltip using the item's data
            string tooltipContent = $"<b>{HeldItem.GetItemName()}</b>\n<size=18>{HeldItem.GetDescription()}</size>";
            TooltipManager.instance.ShowTooltip(tooltipContent);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }

    #endregion
}
