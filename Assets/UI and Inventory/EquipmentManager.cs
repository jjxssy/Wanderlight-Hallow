using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [Header("Equipment Slots")]
    [SerializeField] private EquipmentSlot weaponSlot;
    [SerializeField] private EquipmentSlot helmetSlot;
    [SerializeField] private EquipmentSlot chestplateSlot;
    [SerializeField] private EquipmentSlot legsSlot;
    [SerializeField] private EquipmentSlot accessorySlot1;
    [SerializeField] private EquipmentSlot accessorySlot2;

    [Header("Dependencies")]
    [SerializeField] private ItemDatabase itemDatabase;

    private Dictionary<EquipmentType, EquipmentSlot> _equipmentSlots;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Initialize a dictionary for easy access to slots by type.
        _equipmentSlots = new Dictionary<EquipmentType, EquipmentSlot>
        {
            { EquipmentType.Weapon, weaponSlot },
            { EquipmentType.Helmet, helmetSlot },
            { EquipmentType.Chestplate, chestplateSlot },
            { EquipmentType.Legs, legsSlot },
            // Note: Accessories are handled as a special case.
        };
    }

    public void EquipItem(Item newItem, int fromInventorySlotIndex)
    {
        EquipmentSlot targetSlot = GetSlotForType(newItem.GetEquipmentType());
        if (targetSlot == null) return; // No suitable slot found.

        Item oldItem = targetSlot.HeldItem; // Get the item currently in the slot (could be null).
        targetSlot.SetItem(newItem); // Place the new item in the equipment slot.
        InventoryManager.instance.SwapItemWithSlot(fromInventorySlotIndex, oldItem); // Place the old item back in the inventory.
    }


    public void UnequipItem(Item itemToUnequip)
    {
        // Try to add the item to the main inventory. This will find the first empty slot.
        bool successfullyAdded = InventoryManager.instance.AddItem(itemToUnequip);

        // If it was added successfully (i.e., the inventory wasn't full)...
        if (successfullyAdded)
        {
            // ...find which equipment slot was holding the item and clear it.
            EquipmentSlot sourceSlot = GetSlotHoldingItem(itemToUnequip);
            if (sourceSlot != null)
            {
                sourceSlot.ClearSlot();
            }
        }
        else
        {
            Debug.Log("Inventory is full! Cannot unequip item.");
            // Here you could add feedback to the player, like a sound or a UI message.
        }
    }


    private EquipmentSlot GetSlotForType(EquipmentType type)
    {
        if (type == EquipmentType.Accessory)
        {
            // If the first accessory slot is empty, use it.
            if (accessorySlot1.HeldItem == null) return accessorySlot1;
            // Otherwise, use the second one (this will also handle swaps).
            return accessorySlot2;
        }

        // For all other types, look it up in the dictionary.
        _equipmentSlots.TryGetValue(type, out EquipmentSlot slot);
        return slot;
    }


    private EquipmentSlot GetSlotHoldingItem(Item item)
    {
        if (weaponSlot.HeldItem == item) return weaponSlot;
        if (helmetSlot.HeldItem == item) return helmetSlot;
        if (chestplateSlot.HeldItem == item) return chestplateSlot;
        if (legsSlot.HeldItem == item) return legsSlot;
        if (accessorySlot1.HeldItem == item) return accessorySlot1;
        if (accessorySlot2.HeldItem == item) return accessorySlot2;
        return null;
    }


    public List<EquipmentSaveData> GetDataForSave()
    {
        var data = new List<EquipmentSaveData>();

        // Helper function to add item data to the list
        void AddItemData(EquipmentSlot slot, EquipmentType type)
        {
            if (slot.HeldItem != null)
            {
                data.Add(new EquipmentSaveData { slotType = type, itemName = slot.HeldItem.GetItemName() });
            }
        }

        AddItemData(weaponSlot, EquipmentType.Weapon);
        AddItemData(helmetSlot, EquipmentType.Helmet);
        AddItemData(chestplateSlot, EquipmentType.Chestplate);
        AddItemData(legsSlot, EquipmentType.Legs);
        AddItemData(accessorySlot1, EquipmentType.Accessory); // This will be the first accessory
        AddItemData(accessorySlot2, (EquipmentType)99); // Use a unique key for the second accessory

        return data;
    }

    public void LoadData(List<EquipmentSaveData> equippedItems)
    {
        // Clear all slots first to ensure a clean load
        weaponSlot.ClearSlot();
        helmetSlot.ClearSlot();
        chestplateSlot.ClearSlot();
        legsSlot.ClearSlot();
        accessorySlot1.ClearSlot();
        accessorySlot2.ClearSlot();

        if (equippedItems == null || equippedItems.Count == 0)
        {
            Debug.Log("[Load] No equipment data to load.");
            return;
        }

        Debug.Log($"[Load] Save file contains {equippedItems.Count} equipped item(s). Starting load process.");

        foreach (var itemData in equippedItems)
        {
            if (itemData == null || string.IsNullOrEmpty(itemData.itemName)) continue;

            // --- THIS IS THE KEY LOGGING ---
            Debug.Log($"[Load] Searching database for item with name: '{itemData.itemName}'");
            Item item = itemDatabase.GetItemByName(itemData.itemName);

            if (item == null)
            {
                Debug.LogWarning($"[Load] FAILED to find '{itemData.itemName}' in the ItemDatabase. Skipping this item. Please check for spelling mistakes or ensure the item is in the database asset.");
                continue;
            }

            Debug.Log($"[Load] SUCCESS! Found '{item.GetItemName()}'. Attempting to place in slot for type '{itemData.slotType}'.");
            switch (itemData.slotType)
            {
                case EquipmentType.Weapon: weaponSlot.SetItem(item); break;
                case EquipmentType.Helmet: helmetSlot.SetItem(item); break;
                case EquipmentType.Chestplate: chestplateSlot.SetItem(item); break;
                case EquipmentType.Legs: legsSlot.SetItem(item); break;
                case EquipmentType.Accessory: accessorySlot1.SetItem(item); break;
                case (EquipmentType)99: accessorySlot2.SetItem(item); break; // Second accessory
            }
        }
    }
}

