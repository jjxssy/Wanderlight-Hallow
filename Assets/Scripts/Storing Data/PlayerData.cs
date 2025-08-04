using UnityEngine;

[System.Serializable]
public class PlayerData {

    public int currentHealth;
    public int currentMana;

    public float[] position;

    public PlayerData(PlayerStats stats)
    {
        currentHealth = stats.GetCurrentHealth();
        currentMana = stats.GetCurrentMana();

        position = new float[3];
        position[0] = stats.transform.position.x;
        position[1] = stats.transform.position.y;
        position[2] = stats.transform.position.z;
    }

}
