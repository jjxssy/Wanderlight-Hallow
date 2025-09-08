using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

/// <summary>
/// Handles equipping/unequipping items, applies their stat modifiers to the player,
/// and provides helpers to serialize/deserialize equipped items for saves.
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the <see cref="EquipmentManager"/>.
    /// </summary>
    public static EquipmentManager instance;

    [Header("Equipment Slots")]
    /// <summary>Primary weapon slot reference.</summary>
    [SerializeField] private EquipmentSlot weaponSlot;
    /// <summary>Helmet/hat slot reference.</summary>
    [SerializeField] private EquipmentSlot helmetSlot;
    /// <summary>Chestplate/armor slot reference.</summary>
    [SerializeField] private EquipmentSlot chestplateSlot;
    /// <summary>Legs/greaves slot reference.</summary>
    [SerializeField] private EquipmentSlot legsSlot;
    /// <summary>First accessory slot reference (e.g., ring/amulet).</summary>
    [SerializeField] private EquipmentSlot accessorySlot1;
    /// <summary>Second accessory slot reference.</summary>
    [SerializeField] private EquipmentSlot accessorySlot2;

    [Header("Dependencies")]
    /// <summary>Database used to find items by name when loading saves.</summary>
    [SerializeField] private ItemDatabase itemDatabase;

    /// <summary>
    /// Lookup table for non-accessory equipment slots by <see cref="EquipmentType"/>.
    /// Accessories are handled specially (two separate slots).
    /// </summary>
    private Dictionary<EquipmentType, EquipmentSlot> _equipmentSlots;

    /// <summary>
    /// Initializes the singleton and prepares the slot lookup dictionary.
    /// </summary>
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

    /// <summary>
    /// Equips <paramref name="newItem"/> into its appropriate slot, applies its stat modifiers,
    /// and swaps any previously equipped item back into the inventory.
    /// </summary>
    /// <param name="newItem">Item being equipped.</param>
    /// <param name="fromInventorySlotIndex">Index in inventory the item came from (used when swapping back).</param>
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
        InventoryManager.Instance.SwapItemWithSlot(fromInventorySlotIndex, oldItem);
    }

    /// <summary>
    /// Attempts to unequip <paramref name="itemToUnequip"/>:
    /// adds it to inventory, removes its stat modifiers, and clears its slot on success.
    /// </summary>
    /// <param name="itemToUnequip">Item to remove from its current equipment slot.</param>
    public void UnequipItem(Item itemToUnequip)
    {
        bool successfullyAdded = InventoryManager.Instance.AddItem(itemToUnequip);
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

    /// <summary>
    /// Resolves the appropriate equipment slot for the given <see cref="EquipmentType"/>.
    /// Accessories prefer the first empty accessory slot, otherwise the second.
    /// </summary>
    /// <param name="type">Equipment type to resolve.</param>
    /// <returns>Matching <see cref="EquipmentSlot"/> or <c>null</c> if not found.</returns>
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

    /// <summary>
    /// Finds which slot currently holds the provided <paramref name="item"/>.
    /// </summary>
    /// <param name="item">Item to search for.</param>
    /// <returns>The slot holding the item, or <c>null</c> if none.</returns>
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

    /// <summary>
    /// Builds a list of currently equipped items to be written into a save file.
    /// </summary>
    /// <returns>List of save entries describing equipped items and their slot types.</returns>
    public List<EquipmentSaveData> GetDataForSave()
    {
        var data = new List<EquipmentSaveData>();

        // Helper function to add item data to the list
        void AddItemData(EquipmentSlot slot, EquipmentType type)
        {
            if (slot.HeldItem != null)
            {
                data.Add(new EquipmentSaveData { SlotType = type, ItemName = slot.HeldItem.GetItemName() });
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

    /// <summary>
    /// Restores equipped items from saved data, places them into the correct slots,
    /// and applies their combined stat modifiers to the player.
    /// </summary>
    /// <param name="equippedItems">Saved list of equipped items.</param>
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
            if (itemData == null || string.IsNullOrEmpty(itemData.ItemName)) continue;

            Debug.Log($"[Load] Searching database for item with name: '{itemData.ItemName}'");

            Item item = itemDatabase.GetItemByName(itemData.ItemName);
            if (item == null)
            {
                Debug.LogWarning($"[Load] FAILED to find '{itemData.ItemName}' in the ItemDatabase. Skipping this item. Please check for spelling mistakes or ensure the item is in the database asset.");
                continue;
            }

            totalDefenseFromLoad += item.GetDefenseModifier();
            totalStrengthFromLoad += item.GetStrengthModifier();
            totalMaxHealthFromLoad += item.GetHealthModifier();
            totalSpeedFromLoad += item.GetSpeedModifier();

            Debug.Log($"[Load] SUCCESS! Found '{item.GetItemName()}'. Attempting to place in slot for type '{itemData.SlotType}'.");
            switch (itemData.SlotType)
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
