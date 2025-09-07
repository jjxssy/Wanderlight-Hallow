using UnityEngine;
using System.Collections.Generic;
using System; 

/// <summary>
/// Central runtime manager for achievements.
/// - Holds a catalog of Achievement assets
/// - Tracks progress and unlock state
/// - Persists state via PlayerPrefs
/// - Raises events when progress changes or an achievement is unlocked
///
/// Attach once (e.g., in a bootstrap scene) and it will persist across scenes.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the manager.
    /// </summary>
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
    }

    /// <summary>
    /// Build lookup dictionary, clear in-memory state, and load persisted progress.
    /// </summary>
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

    /// <summary>
    /// Adds progress to the achievement with the given ID.
    /// Persists the change and fires progress/unlock events.
    /// </summary>
    /// <param name="achievementId">Unique achievement ID.</param>
    /// <param name="amount">Amount to add (can be negative, but usually positive).</param>
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

    /// <summary>
    /// Unlocks the provided achievement, fires events, and persists state.
    /// </summary>
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

    /// <summary>
    /// Persists a single achievement's progress and unlock state to PlayerPrefs.
    /// </summary>
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

        // The 'if' block that was causing the problem has been removed from here.
        if (PlayerPrefs.HasKey(unlockedKey))
        {
            achievement.isUnlocked = PlayerPrefs.GetInt(unlockedKey) == 1;
        }
    }

    /// <summary>
    /// Resets a single achievement's progress/unlock state and persists.
    /// </summary>
    /// <param name="achievementId">Unique achievement ID.</param>
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


    /// <summary>
    /// Resets all achievements and persists.
    /// </summary>
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
    /// <summary>
    /// Returns a specific achievement by ID, or null if not found.
    /// </summary>
    public List<Achievement> GetAchievements()
    {
        return achievements;
    }

    #endregion
}