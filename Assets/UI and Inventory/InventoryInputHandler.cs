using UnityEngine;
using UnityEngine.UI;

public class InventoryInputHandler : MonoBehaviour
{
    [SerializeField] private KeyBindingsManager keyBindingsManager;
    [SerializeField] private InventoryManager inventoryManager; // 👈 NEW
    [SerializeField] private Image[] slotHighlights;

    private int currentSlot = -1;

    private void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            string quickslotAction = $"QUICKSLOT {i + 1}";
            if (keyBindingsManager == null || slotHighlights.Length < 5) return;

            if (Input.GetKeyDown(keyBindingsManager.GetKey(quickslotAction)))
            {
                SelectSlot(i);
                break;
            }
        }
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= slotHighlights.Length) return;

        if (currentSlot != -1)
            slotHighlights[currentSlot].enabled = false;

        slotHighlights[index].enabled = true;
        currentSlot = index;

        Debug.Log($"Selected quickslot {index + 1}");

        // Notify the inventory manager (if needed)
        inventoryManager?.OnSlotSelected(index);
    }
}
