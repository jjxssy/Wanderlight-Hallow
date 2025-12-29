using UnityEngine;
using System.Collections.Generic; // Required for List

/// <summary>
/// Represents equipment data for saving (slot + item name).
/// </summary>
[System.Serializable]
public class EquipmentSaveData
{
    private EquipmentType slotType;
    private string itemName;

    public EquipmentType SlotType
    {
        get => slotType;
        set => slotType = value;
    }

    public string ItemName
    {
        get => itemName;
        set => itemName = value;
    }
}

/// <summary>
/// Stores all data needed to save and load a player.
/// Includes stats, inventory, destroyed world items, and equipped items.
/// </summary>
[System.Serializable]
public class PlayerData
{
    // --- Core Player Data ---
    private int currentHealth;
    private int currentMana;
    private float[] position;
    private bool isTutorialComplete = false;

    // --- Inventory and Equipment ---
    private string[] inventoryItemNames; 
    private List<string> destroyedWorldItemIds;
    private List<EquipmentSaveData> equippedItems;

    /// <summary>
    /// Constructor that captures the current state of the player.
    /// </summary>
    public PlayerData(PlayerStats stats, InventoryManager invManager, WorldItemManager itemManager, EquipmentManager equipManager)
    {
        // Save existing stats
        currentHealth = stats.GetCurrentHealth();
        currentMana = stats.GetCurrentMana();
        position = new float[3];
        position[0] = stats.transform.position.x;
        position[1] = stats.transform.position.y;
        position[2] = stats.transform.position.z;

        // Save additional data
        inventoryItemNames = invManager.GetInventoryDataForSave();
        destroyedWorldItemIds = itemManager.GetDestroyedItemIdsForSave();
        equippedItems = equipManager.GetDataForSave();
    }

    // --- Simple Getters & Setters ---

    public bool GetStatus() { return isTutorialComplete;}
    public void SetStatus(bool status) { isTutorialComplete = status; }
    public int Health
    {
        get => currentHealth;
        set => currentHealth = value;
    }

    public int Mana
    {
        get => currentMana;
        set => currentMana = value;
    }

    public float[] Position
    {
        get => position;
        set => position = value;
    }

    public string[] InventoryItemNames
    {
        get => inventoryItemNames;
        set => inventoryItemNames = value;
    }

    public List<string> DestroyedWorldItemIds
    {
        get => destroyedWorldItemIds;
        set => destroyedWorldItemIds = value;
    }

    public List<EquipmentSaveData> EquippedItems
    {
        get => equippedItems;
        set => equippedItems = value;
    }
}
