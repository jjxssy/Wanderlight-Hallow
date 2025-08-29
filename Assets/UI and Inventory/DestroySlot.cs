using UnityEngine;
using UnityEngine.EventSystems;

public class DestroySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot sourceSlot = InventorySlot.DraggedSlot;

        if (sourceSlot != null && sourceSlot.HeldItem != null)
        {
            Debug.Log($"Destroyed item: {sourceSlot.HeldItem.GetItemName()}");

            InventoryManager.instance.DestroyItem(sourceSlot.SlotIndex);
        }
    }
}
