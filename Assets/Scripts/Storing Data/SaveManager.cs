using UnityEngine;

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


    private PlayerData data;


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


    public void DeleteSave(int slotIndex)
    {
        SaveSystem.DeletePlayer(slotIndex);
        Debug.Log($"Save file for slot {slotIndex} deleted.");
    }

    private void LoadPlayer()
    {
        // Load player stats
        stats.SetCurrentHealth(data.getHealth());
        stats.SetCurrentMana(data.getMana());
        Vector3 position;
        position.x = data.getPosition()[0];
        position.y = data.getPosition()[1];
        position.z = data.getPosition()[2];
        stats.transform.position = position;

        // Load inventory and world item states
        inventoryManager.LoadInventoryData(data.inventoryItemNames);
        worldItemManager.LoadDestroyedItems(data.destroyedWorldItemIds);
        equipmentManager.LoadData(data.equippedItems);

        Debug.Log("Player data loaded successfully.");
    }
}
