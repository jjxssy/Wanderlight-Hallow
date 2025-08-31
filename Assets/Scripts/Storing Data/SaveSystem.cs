using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    // A single save method that takes a slot index and all required data managers
    public static void SavePlayer(PlayerStats stats, InventoryManager invManager, WorldItemManager itemManager, EquipmentManager equipManager, int slotIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPathFromSlotIndex(slotIndex);
        FileStream stream = new FileStream(path, FileMode.Create);

        // This line now correctly passes all three required arguments
        PlayerData data = new PlayerData(stats, invManager, itemManager, equipManager);

        formatter.Serialize(stream, data);
        stream.Close();

        PlayerPrefs.SetInt("SavedLevel" + slotIndex, 1);
    }

    // A single load method
    public static PlayerData LoadPlayer(int slotIndex)
    {
        string path = GetPathFromSlotIndex(slotIndex);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    // A single delete method
    public static void DeletePlayer(int slotIndex)
    {
        string path = GetPathFromSlotIndex(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        PlayerPrefs.DeleteKey("SavedLevel" + slotIndex);
    }

    // Helper method to get the file path based on slot number
    private static string GetPathFromSlotIndex(int slotIndex)
    {
        return Application.persistentDataPath + "/player.save" + slotIndex;
    }
}