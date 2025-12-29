using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Static system for saving, loading, and deleting player data
/// across multiple save slots using binary serialization.
/// </summary>
public static class SaveSystem
{
    /// <summary>
    /// Saves the current player state into the given slot index.
    /// </summary>
    /// <param name="stats">The player's stats manager.</param>
    /// <param name="invManager">The inventory manager.</param>
    /// <param name="itemManager">The world item manager.</param>
    /// <param name="equipManager">The equipment manager.</param>
    /// <param name="slotIndex">The save slot index (1, 2, 3, etc.).</param>
    public static void SavePlayer(PlayerStats stats, InventoryManager invManager, WorldItemManager itemManager, EquipmentManager equipManager, int slotIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPathFromSlotIndex(slotIndex);
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(stats, invManager, itemManager, equipManager);
        int temp = PlayerPrefs.GetInt("Tutorial" + slotIndex, 0);
        if (temp == 1) data.SetStatus(true);
        else data.SetStatus(false);

        formatter.Serialize(stream, data);
        stream.Close();

        PlayerPrefs.SetInt("SavedLevel" + slotIndex, 1);
    }

    /// <summary>
    /// Loads a player’s saved data from the given slot index.
    /// </summary>
    /// <param name="slotIndex">The save slot index to load from.</param>
    /// <returns>The deserialized <see cref="PlayerData"/>, or <c>null</c> if no save exists.</returns>
    public static PlayerData LoadPlayer(int slotIndex)
    {
        string path = GetPathFromSlotIndex(slotIndex);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            int temp = PlayerPrefs.GetInt("Tutorial" + slotIndex, 0);
            if (temp == 1) PlayerPrefs.SetInt("LoadedTutorial", 1);
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    /// <summary>
    /// Deletes the player’s saved data at the given slot index.
    /// </summary>
    /// <param name="slotIndex">The save slot index to delete.</param>
    public static void DeletePlayer(int slotIndex)
    {
        string path = GetPathFromSlotIndex(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        PlayerPrefs.DeleteKey("SavedLevel" + slotIndex);
    }

    /// <summary>
    /// Gets the file path for a given save slot index.
    /// </summary>
    /// <param name="slotIndex">The slot index.</param>
    /// <returns>A string file path inside <see cref="Application.persistentDataPath"/>.</returns>
    private static string GetPathFromSlotIndex(int slotIndex)
    {
        return Application.persistentDataPath + "/player.save" + slotIndex;
    }
}
