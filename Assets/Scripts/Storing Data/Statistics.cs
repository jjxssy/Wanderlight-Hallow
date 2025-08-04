using UnityEngine;

public static class Statistics 
{
    //===== All Stats =====//
    //Death
    //Kills
    //Score
    //Achievements
    //==== You can add other Statistics Here ====//

    public static int GetDeathStats()
    {
        return PlayerPrefs.GetInt("DeathStats", 0);
    }
    public static void IncreaseDeathStats()
    {
        PlayerPrefs.SetInt("DeathStats", GetDeathStats() + 1);
    }

    public static int GetKillStats()
    {
        return PlayerPrefs.GetInt("KillStats", 0);
    }
    public static void IncreaseKillStats()
    {
        PlayerPrefs.SetInt("KillStats", GetKillStats()+1);
    }

    public static int GetScoreStats()
    {
        return PlayerPrefs.GetInt("ScoreStats", 0);
    }
    public static void IncreaseScoreStats(int amount)
    {
         PlayerPrefs.SetInt("ScoreStats", GetScoreStats() + amount);
    }

    public static int GetAchievementStats()
    {
        return PlayerPrefs.GetInt("AchievementStats", 0);
    }
    public static void IncreaseAchievementStats()
    {
        PlayerPrefs.GetInt("AchievementStats", GetAchievementStats() + 1);
    }

    public static void ClearAllStats()
    {
        PlayerPrefs.DeleteAll();
    }


}
