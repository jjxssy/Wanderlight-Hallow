using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

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
        if (targetSlot == null) return; 

        Item oldItem = targetSlot.HeldItem;
        if (oldItem != null)
        {
            PlayerStats.instance.SetDefense(PlayerStats.instance.GetDefense()-oldItem.GetDefenseModifier());
            PlayerStats.instance.SetStrength(PlayerStats.instance.GetStrength()-oldItem.GetStrengthModifier());
            PlayerStats.instance.SetMaxHealth(PlayerStats.instance.GetMaxHealth()-oldItem.GetHealthModifier());
            PlayerStats.instance.SetSpeed(PlayerStats.instance.GetSpeed()-oldItem.GetSpeedModifier());
        }
        PlayerStats.instance.SetDefense(PlayerStats.instance.GetDefense() + newItem.GetDefenseModifier());
        PlayerStats.instance.SetStrength(PlayerStats.instance.GetStrength() + newItem.GetStrengthModifier());
        PlayerStats.instance.SetMaxHealth(PlayerStats.instance.GetMaxHealth() + newItem.GetHealthModifier());
        PlayerStats.instance.SetSpeed(PlayerStats.instance.GetSpeed() + newItem.GetSpeedModifier());
        targetSlot.SetItem(newItem);
        InventoryManager.instance.SwapItemWithSlot(fromInventorySlotIndex, oldItem);
    }


    public void UnequipItem(Item itemToUnequip)
    {
        bool successfullyAdded = InventoryManager.instance.AddItem(itemToUnequip);
        if (successfullyAdded)
        {
            PlayerStats.instance.SetDefense(PlayerStats.instance.GetDefense() - itemToUnequip.GetDefenseModifier());
            PlayerStats.instance.SetStrength(PlayerStats.instance.GetStrength() - itemToUnequip.GetStrengthModifier());
            PlayerStats.instance.SetMaxHealth(PlayerStats.instance.GetMaxHealth() - itemToUnequip.GetHealthModifier());
            PlayerStats.instance.SetSpeed(PlayerStats.instance.GetSpeed() - itemToUnequip.GetSpeedModifier());

            EquipmentSlot sourceSlot = GetSlotHoldingItem(itemToUnequip);
            if (sourceSlot != null)
            {
                sourceSlot.ClearSlot();
            }
        }
        else
        {
            Debug.Log("Inventory is full! Cannot unequip item.");
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

        int totalDefenseFromLoad = 0;
        int totalStrengthFromLoad = 0;
        int totalMaxHealthFromLoad = 0;
        int totalSpeedFromLoad = 0;

        foreach (var itemData in equippedItems)
        {
            if (itemData == null || string.IsNullOrEmpty(itemData.itemName)) continue;

            Debug.Log($"[Load] Searching database for item with name: '{itemData.itemName}'");

            Item item = itemDatabase.GetItemByName(itemData.itemName);
            if (item == null)
            {
                Debug.LogWarning($"[Load] FAILED to find '{itemData.itemName}' in the ItemDatabase. Skipping this item. Please check for spelling mistakes or ensure the item is in the database asset.");
                continue;
            }

            totalDefenseFromLoad += item.GetDefenseModifier();
            totalStrengthFromLoad += item.GetStrengthModifier();
            totalMaxHealthFromLoad += item.GetHealthModifier();
            totalSpeedFromLoad += item.GetSpeedModifier();

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
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.SetDefense(PlayerStats.instance.GetDefense() + totalDefenseFromLoad);
            PlayerStats.instance.SetStrength(PlayerStats.instance.GetStrength() + totalStrengthFromLoad);
            PlayerStats.instance.SetMaxHealth(PlayerStats.instance.GetMaxHealth() + totalMaxHealthFromLoad);
            PlayerStats.instance.SetSpeed(PlayerStats.instance.GetSpeed() + totalSpeedFromLoad);
        }
    }
}

