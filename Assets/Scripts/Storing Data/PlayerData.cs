using UnityEngine;
using System.Collections.Generic; // Required for List

[System.Serializable]
public class EquipmentSaveData
{
    public EquipmentType slotType;
    public string itemName;
}
[System.Serializable]
public class PlayerData
{
    // Existing Data
    private int currentHealth;
    private int currentMana;
    private float[] position;

    // --- New Data ---
    public string[] inventoryItemNames; 
    public List<string> destroyedWorldItemIds;
    public List<EquipmentSaveData> equippedItems;

    public PlayerData(PlayerStats stats, InventoryManager invManager, WorldItemManager itemManager, EquipmentManager equipManager)
    {
        // Save existing stats
        currentHealth = stats.GetCurrentHealth();
        currentMana = stats.GetCurrentMana();
        position = new float[3];
        position[0] = stats.transform.position.x;
        position[1] = stats.transform.position.y;
        position[2] = stats.transform.position.z;

        // --- Save New Data ---
        inventoryItemNames = invManager.GetInventoryDataForSave();
        destroyedWorldItemIds = itemManager.GetDestroyedItemIdsForSave();
        equippedItems = equipManager.GetDataForSave();
    }

    // --- Getters and Setters (no changes needed for old ones) ---
    public int getHealth() { return currentHealth; }
    public int getMana() { return currentMana; }
    public float[] getPosition() { return position; }
    public void setHealth(int health) { currentHealth = health; }
    public void setMana(int mana) { currentMana = mana; }
    public void setPosition(float[] pos) { this.position = pos; }
}
