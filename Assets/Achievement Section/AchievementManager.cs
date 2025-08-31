using UnityEngine;
using System.Collections.Generic;
using System; 

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [Tooltip("A list of all achievements in the game.")]
    [SerializeField] private List<Achievement> achievements;


    private Dictionary<string, Achievement> achievementDictionary = new Dictionary<string, Achievement>();

    public static event Action<Achievement> OnAchievementUnlocked;
    public static event Action<Achievement> OnProgressUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); 
        InitializeAchievements();
        SaveSystem.DeletePlayer(0);
    }

    private void InitializeAchievements()
    {
        foreach (Achievement achievement in achievements)
        {
            achievement.isUnlocked = false;
            achievement.currentProgress = 0;

            if (!achievementDictionary.ContainsKey(achievement.id))
            {
                achievementDictionary.Add(achievement.id, achievement);
            }

            LoadAchievementProgress(achievement);
        }
        Debug.Log("Achievement Manager Initialized.");
    }

    public void AddProgress(string achievementId, int amount)
    {
        if (achievementDictionary.TryGetValue(achievementId, out Achievement achievement))
        {
            if (achievement.isUnlocked) return;

            achievement.currentProgress += amount;

            OnProgressUpdated?.Invoke(achievement);
            if (achievement.currentProgress >= achievement.goal)
            {
                UnlockAchievement(achievement);
            }

            SaveAchievementProgress(achievement);
        }
        else
        {
            Debug.LogWarning($"Achievement with ID '{achievementId}' not found!");
        }
    }

    private void UnlockAchievement(Achievement achievement)
    {
        if (achievement.isUnlocked) return;

        achievement.isUnlocked = true;
        achievement.currentProgress = achievement.goal;
        Debug.Log($"Achievement Unlocked: {achievement.title}");

        OnAchievementUnlocked?.Invoke(achievement);
        SaveAchievementProgress(achievement);
    }

    #region Saving and Loading

    private void SaveAchievementProgress(Achievement achievement)
    {
        string progressKey = $"Achievement_{achievement.id}_Progress";
        string unlockedKey = $"Achievement_{achievement.id}_Unlocked";

        PlayerPrefs.SetInt(progressKey, achievement.currentProgress);
        PlayerPrefs.SetInt(unlockedKey, achievement.isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadAchievementProgress(Achievement achievement)
    {
        string progressKey = $"Achievement_{achievement.id}_Progress";
        string unlockedKey = $"Achievement_{achievement.id}_Unlocked";

        if (PlayerPrefs.HasKey(progressKey))
        {
            achievement.currentProgress = PlayerPrefs.GetInt(progressKey);
        }

        if (PlayerPrefs.HasKey(unlockedKey))
        {
            achievement.isUnlocked = PlayerPrefs.GetInt(unlockedKey) == 1;
        }
    }
    public void ResetAchievement(string achievementId)
    {
        if (achievementDictionary.TryGetValue(achievementId, out Achievement achievement))
        {
            achievement.currentProgress = 0;
            achievement.isUnlocked = false;

            SaveAchievementProgress(achievement);
            OnProgressUpdated?.Invoke(achievement);

            Debug.Log($"Achievement '{achievement.title}' has been reset.");
        }
        else
        {
            Debug.LogWarning($"Could not reset achievement. ID '{achievementId}' not found!");
        }
    }

    public void ResetAllAchievements()
    {
        foreach (Achievement achievement in achievements)
        {
            achievement.currentProgress = 0;
            achievement.isUnlocked = false;
            SaveAchievementProgress(achievement);
            OnProgressUpdated?.Invoke(achievement);
        }

        Debug.Log("All achievements have been reset.");
    }
    public List<Achievement> GetAchievements()
    {
        return achievements;
    }

    #endregion
}