using UnityEngine;

[System.Serializable]
public class GameData
{
    public int deathCount;

    // initial values when starting a game
    public GameData()
    { 
        this.deathCount = 0;
    }
}
