using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Button> slotButtons; // Assign in Inspector
    [SerializeField] private Sprite defaultSlotSprite; // Assign empty icon in Inspector

    private void Start()
    {
        ClearInventory();
    }

    public void ClearInventory()
    {
        foreach (Button btn in slotButtons)
        {
            btn.image.sprite = defaultSlotSprite;
        }
    }

    public void AddItem(Sprite itemSprite)
    {
        foreach (Button btn in slotButtons)
        {
            if (btn.image.sprite == defaultSlotSprite)
            {
                btn.image.sprite = itemSprite;
                return;
            }
        }

        Debug.Log("Inventory Full");
    }

    public void OnSlotSelected(int index)
    {
        Debug.Log($"InventoryManager: Slot {index + 1} selected");

        // 🧠 Placeholder for item usage logic (can be expanded later)
    }
}
