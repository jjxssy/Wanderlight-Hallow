using UnityEngine;
using UnityEngine.EventSystems;

public class QuickslotDrop : MonoBehaviour, IDropHandler
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

        Debug.Log($"Drop: {dragged.ItemData.ItemName} from {dragged.TypeOfSlot} {dragged.SlotIndex} → quickslot {slotIndex}");

        // Corrected Logic
        if (dragged.TypeOfSlot == SlotType.Inventory)
        {
            inventoryManager.AssignItemToSlot(slotIndex, dragged.ItemData);
            inventoryManager.ClearInventorySlot(dragged.SlotIndex);
        }
        else if (dragged.TypeOfSlot == SlotType.Quickslot && dragged.SlotIndex != slotIndex)
        {
            inventoryManager.SwapQuickslots(dragged.SlotIndex, slotIndex);
        }
    }
}
