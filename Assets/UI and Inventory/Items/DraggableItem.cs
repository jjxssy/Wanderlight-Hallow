using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Image iconImage;
    private Canvas canvas;
    private Transform originalParent;

    private Item itemData;
    private int slotIndex = -1;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
        iconImage.raycastTarget = true;
        iconImage.enabled = false;
    }

    /// <summary>
    /// Sets the item and updates the icon based on its data.
    /// </summary>
    public void SetItem(Item item, int slotIndex = -1)
    {
        itemData = item;
        this.slotIndex = slotIndex;

        iconImage.sprite = item != null ? item.GetIcon() : null;
        iconImage.enabled = item != null;
    }

    /// <summary>
    /// Sets the reference to the inventory manager.
    /// </summary>
    public void SetInventoryManager(InventoryManager manager)
    {
        inventoryManager = manager;
    }

    /// <summary>
    /// Returns the assigned item.
    /// </summary>
    public Item GetItemData()
    {
        return itemData;
    }

    /// <summary>
    /// Returns the slot index of this item.
    /// </summary>
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemData == null) return;
        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
        iconImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemData == null) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent, false);
        transform.localPosition = Vector3.zero;
        iconImage.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right &&
            itemData != null &&
            inventoryManager != null)
        {
            inventoryManager.OnSlotSelected(slotIndex);
        }
    }

    /// <summary>
    /// Checks if the item is from the quickslot bar.
    /// </summary>
    public bool IsFromQuickslot()
    {
        return slotIndex >= 0 && slotIndex < inventoryManager.GetQuickslotCount();
    }

    /// <summary>
    /// Checks if the item is from the main inventory grid.
    /// </summary>
    public bool IsFromInventory()
    {
        return slotIndex >= 0 && slotIndex < inventoryManager.GetInventorySlotCount();
    }
}
