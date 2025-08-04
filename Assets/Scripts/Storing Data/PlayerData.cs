using UnityEngine;

[System.Serializable]
public class PlayerData {

    private int currentHealth;
    private int currentMana;

    private float[] position;

    public PlayerData(PlayerStats stats)
    {
        currentHealth = stats.GetCurrentHealth();
        currentMana = stats.GetCurrentMana();

        position = new float[3];
        position[0] = stats.transform.position.x;
        position[1] = stats.transform.position.y;
        position[2] = stats.transform.position.z;
    }
    public int getHealth() { return currentHealth; }
    public int getMana() { return currentMana; }
    public float[] getPosition() { return position; }
    public void setHealth(int health) { currentHealth = health; }
    public void setMana(int mana) { currentMana = mana; }
    public void setPosition(float[] pos) {this.position = pos; }

}
