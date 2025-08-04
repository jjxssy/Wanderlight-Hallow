using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    public static void SavePlayer1(PlayerStats stats)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.firstSave";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(stats);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer1()
    {
        string path = Application.persistentDataPath + "/player.firstSave";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();
            return data;

        }
        else
        {
            Debug.LogError("Save file not found in " +  path);
            return null;
        }
    }

    public static void SavePlayer2(PlayerStats stats)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.secondSave";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(stats);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer2()
    {
        string path = Application.persistentDataPath + "/player.secondSave";
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

        public static void SavePlayer3(PlayerStats stats)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.thirdSave";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(stats);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer3()
    {
        string path = Application.persistentDataPath + "/player.thirdSave";
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
}
