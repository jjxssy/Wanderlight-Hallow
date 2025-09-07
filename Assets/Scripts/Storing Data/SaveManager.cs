using UnityEngine;

/// <summary>
/// Handles saving, loading, and deleting player save files,
/// coordinating PlayerStats, Inventory, World items, and Equipment.
/// </summary>
public class SaveManager : MonoBehaviour
{
    [Header("Required References")]
    [Tooltip("Reference to the player's stats component.")]
    [SerializeField] private PlayerStats stats;

    [Tooltip("Reference to the inventory manager.")]
    [SerializeField] private InventoryManager inventoryManager;

    [Tooltip("Reference to the world item manager.")]
    [SerializeField] private WorldItemManager worldItemManager;

    [Tooltip("Reference to the equipment manager.")]
    [SerializeField] private EquipmentManager equipmentManager;

    /// <summary>
    /// In-memory copy of the currently loaded player data.
    /// </summary>
    private PlayerData data;

    /// <summary>
    /// On start, if LoadIndex != 0, attempts to load the selected save.
    /// </summary>
    private void Start()
    {
        int loadIndex = PlayerPrefs.GetInt("LoadIndex", 0);
        if (loadIndex == 0) return;

        data = SaveSystem.LoadPlayer(loadIndex);
        if (data != null)
        {
            LoadPlayer();
        }
    }

    /// <summary>
    /// Saves the game into a given slot index.
    /// </summary>
    /// <param name="slotIndex">Target save slot.</param>
    public void SaveGame(int slotIndex)
    {
        if (stats == null || inventoryManager == null || worldItemManager == null)
        {
            Debug.LogError("Save Manager is missing required references!");
            return;
        }

        SaveSystem.SavePlayer(stats, inventoryManager, worldItemManager, equipmentManager, slotIndex);
        Debug.Log($"Game saved to slot {slotIndex}.");
    }

    /// <summary>
    /// Deletes the save file at the given slot index.
    /// </summary>
    /// <param name="slotIndex">Target save slot to delete.</param>
    public void DeleteSave(int slotIndex)
    {
        SaveSystem.DeletePlayer(slotIndex);
        Debug.Log($"Save file for slot {slotIndex} deleted.");
    }

    /// <summary>
    /// Applies loaded <see cref="PlayerData"/> to runtime systems.
    /// </summary>
    private void LoadPlayer()
    {
        // Load player stats
        stats.SetCurrentHealth(data.Health);
        stats.SetCurrentMana(data.Mana);

        Vector3 position;
        var posArray = data.Position; // float[3] { x, y, z }
        position.x = posArray[0];
        position.y = posArray[1];
        position.z = posArray[2];
        stats.transform.position = position;

        // Load inventory and world item states
        worldItemManager.LoadDestroyedItems(data.DestroyedWorldItemIds);
        inventoryManager.LoadInventoryData(data.InventoryItemNames);
        equipmentManager.LoadData(data.EquippedItems);

        Debug.Log("Player data loaded successfully.");
    }
}
