using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Image iconImage;
    private Canvas canvas;
    private Transform originalParent;

    public Item ItemData { get; private set; }
    public int SlotIndex { get; private set; } = -1;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
        iconImage.raycastTarget = true;
        iconImage.enabled = false;
    }

    public void SetItem(Item item, int slotIndex = -1)
    {
        ItemData = item;
        SlotIndex = slotIndex;

        iconImage.sprite = item != null ? item.Icon : null;
        iconImage.enabled = item != null;
    }

    public void SetInventoryManager(InventoryManager manager)
    {
        inventoryManager = manager;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemData == null) return;
        originalParent = transform.parent;
        transform.SetParent(canvas.transform, true);
        iconImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ItemData == null) return;
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
            ItemData != null &&
            inventoryManager != null)
        {
            inventoryManager.OnSlotSelected(SlotIndex);
        }
    }

    public bool IsFromQuickslot() => SlotIndex >= 0 && SlotIndex < inventoryManager.QuickslotCount;
    public bool IsFromInventory() => SlotIndex >= 0 && SlotIndex < inventoryManager.InventorySlotCount;
}
