using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField]private PlayerStats stats;
    private PlayerData data;
    private void Awake()
    {
        if (PlayerPrefs.GetInt("LoadIndex",0) == 0) return;
        if (PlayerPrefs.GetInt("LoadIndex", 0) == 1) data = SaveSystem.LoadPlayer1();
        if (PlayerPrefs.GetInt("LoadIndex", 0) == 2) data = SaveSystem.LoadPlayer2();
        if (PlayerPrefs.GetInt("LoadIndex", 0) == 3) data = SaveSystem.LoadPlayer3();
        LoadPlayer();
    }
    public void SavePlayer1()
    {
        SaveSystem.SavePlayer1(stats);
    }
    public void DeletePlayer1()
    {
        SaveSystem.DeletePlayer1();
    }
    public void SavePlayer2()
    {
        SaveSystem.SavePlayer2(stats);
    }
    public void DeletePlayer2()
    {
        SaveSystem.DeletePlayer2();
    }
    public void SavePlayer3()
    {
        SaveSystem.SavePlayer3(stats);
    }
    public void DeletePlayer3()
    {
        SaveSystem.DeletePlayer3();
    }
    private void LoadPlayer()
    {
        stats.SetCurrentHealth(data.getHealth());
        stats.SetCurrentMana(data.getMana());
        Vector3 position;
        position.x = data.getPosition()[0];
        position.y = data.getPosition()[1];
        position.z = data.getPosition()[2];
        stats.transform.position = position;

    }
}
