using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerStats stats;

    public void SavePlayer()
    {
        SaveSystem.SavePlayer1(stats);
    }
    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer1();
        stats.SetCurrentHealth(data.currentHealth);
        stats.SetCurrentMana(data.currentMana);
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        stats.transform.position = position;

    }
}
