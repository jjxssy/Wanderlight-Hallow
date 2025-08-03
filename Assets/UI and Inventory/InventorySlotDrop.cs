using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDrop : MonoBehaviour, IDropHandler
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private int slotIndex;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem dragged = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (dragged == null || dragged.ItemData == null) return;

        switch (dragged.TypeOfSlot)
        {
            case SlotType.Inventory:
                if (dragged.SlotIndex != slotIndex)
                {
                    inventoryManager.SwapInventoryItems(dragged.SlotIndex, slotIndex);
                }
                break;

            case SlotType.Quickslot:
                inventoryManager.SwapQuickslotInventory(dragged.SlotIndex, slotIndex);
                break;
        }
    }
}