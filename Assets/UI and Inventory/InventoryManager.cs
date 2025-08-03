using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Quickslots")]
    [SerializeField] private List<Button> slotButtons;
    [SerializeField] private List<DraggableItem> slotIcons;
    [SerializeField] private Sprite defaultSlotSprite;
    [SerializeField] private List<Item> items = new List<Item>(5);

    [Header("Inventory")]
    [SerializeField] private List<DraggableItem> inventoryIcons;
    [SerializeField] private List<Item> inventoryItems;

    [Header("Keybinds (optional)")]
    [SerializeField] private KeyBindingsManager keyBindingsManager;

    public int QuickslotCount => slotButtons.Count;
    public int InventorySlotCount => inventoryIcons.Count;

    private void Awake()
    {
        while (items.Count < slotButtons.Count)
            items.Add(null);

        while (inventoryItems.Count < inventoryIcons.Count)
            inventoryItems.Add(null);
    }

    private void Start()
    {
        for (int i = 0; i < slotButtons.Count; i++)
        {
            int index = i;
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSlotSelected(index));
            slotIcons[i].SetItem(items[i], index, SlotType.Quickslot);
            slotIcons[i].SetInventoryManager(this);
        }

        for (int i = 0; i < inventoryIcons.Count; i++)
        {
            inventoryIcons[i].SetItem(inventoryItems[i], i, SlotType.Inventory);
            inventoryIcons[i].SetInventoryManager(this);
        }
    }

    private void Update()
    {
        if (keyBindingsManager == null) return;

        for (int i = 0; i < slotButtons.Count; i++)
        {
            string actionName = $"QUICKSLOT {i + 1}";
            KeyCode key = keyBindingsManager.GetKey(actionName);
            if (Input.GetKeyDown(key))
            {
                OnSlotSelected(i);
            }
        }
    }

    public void OnSlotSelected(int index)
    {
        if (index < 0 || index >= items.Count) return;

        Item item = items[index];
        if (item == null)
        {
            Debug.Log($"Slot {index + 1} is empty.");
            return;
        }

        if (item.GetItemType() == ItemType.Consumable)
        {
            Debug.Log($"Consumed {item.GetItemName()} from quickslot {index + 1}");
            ClearQuickslot(index);
        }
        else
        {
            Debug.Log($"Equipped {item.GetItemName()} from quickslot {index + 1}");
        }
    }

    public void AssignItemToSlot(int index, Item item)
    {
        if (index < 0 || index >= items.Count) return;
        Debug.Log("---" + index);
        items[index] = item;

        slotIcons[index].SetItem(item, index,SlotType.Quickslot);
        slotIcons[index].SetInventoryManager(this);
    }

    public void ClearQuickslot(int index)
    {
        if (index < 0 || index >= items.Count) return;

        items[index] = null;
        slotButtons[index].image.sprite = defaultSlotSprite;
        slotIcons[index].SetItem(null, index,SlotType.Quickslot);
    }

    public void AssignInventoryItem(int index, Item item)
    {
        if (index < 0 || index >= inventoryItems.Count) return;

        inventoryItems[index] = item;
        inventoryIcons[index].SetItem(item, index, SlotType.Inventory);
        inventoryIcons[index].SetInventoryManager(this);
    }

    public void SwapInventoryItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= inventoryItems.Count || indexB >= inventoryItems.Count) return;

        (inventoryItems[indexA], inventoryItems[indexB]) = (inventoryItems[indexB], inventoryItems[indexA]);

        inventoryIcons[indexA].SetItem(inventoryItems[indexA], indexA, SlotType.Inventory);
        inventoryIcons[indexB].SetItem(inventoryItems[indexB], indexB, SlotType.Inventory);

        inventoryIcons[indexA].SetInventoryManager(this);
        inventoryIcons[indexB].SetInventoryManager(this);
    }
    
    public void ClearInventorySlot(int index)
    {
        if (index < 0 || index >= inventoryItems.Count) return;

        inventoryItems[index] = null;
        inventoryIcons[index].SetItem(null, index, SlotType.Inventory);
    }

    public void SwapQuickslots(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= items.Count || indexB >= items.Count)
            return;

        (items[indexA], items[indexB]) = (items[indexB], items[indexA]);

        slotIcons[indexA].SetItem(items[indexA], indexA, SlotType.Quickslot);
        slotIcons[indexB].SetItem(items[indexB], indexB, SlotType.Quickslot);

        slotIcons[indexA].SetInventoryManager(this);
        slotIcons[indexB].SetInventoryManager(this);
    }

    public void SwapQuickslotInventory(int quickslotIndex, int inventoryIndex)
    {
        Debug.Log($"Attempting to swap Quickslot[{quickslotIndex}] with Inventory[{inventoryIndex}].");

        if (quickslotIndex < 0 || quickslotIndex >= items.Count ||
            inventoryIndex < 0 || inventoryIndex >= inventoryItems.Count)
        {
            Debug.LogError("Swap failed: One or both slot indices were out of bounds.");
            return;
        }

        string quickslotItemName = items[quickslotIndex] != null ? items[quickslotIndex].GetItemName() : "Empty";
        string inventoryItemName = inventoryItems[inventoryIndex] != null ? inventoryItems[inventoryIndex].GetItemName() : "Empty";
        Debug.Log($"Before swap: Quickslot has '{quickslotItemName}', Inventory has '{inventoryItemName}'.");

        (inventoryItems[inventoryIndex], items[quickslotIndex]) = (items[quickslotIndex], inventoryItems[inventoryIndex]);

        inventoryIcons[inventoryIndex].SetItem(inventoryItems[inventoryIndex], inventoryIndex, SlotType.Inventory);
        slotIcons[quickslotIndex].SetItem(items[quickslotIndex], quickslotIndex, SlotType.Quickslot);

        quickslotItemName = items[quickslotIndex] != null ? items[quickslotIndex].GetItemName() : "Empty";
        inventoryItemName = inventoryItems[inventoryIndex] != null ? inventoryItems[inventoryIndex].GetItemName() : "Empty";
        Debug.Log($"After swap: Quickslot now has '{inventoryItemName}', Inventory now has '{quickslotItemName}'. Swap successful!");
    }


}
