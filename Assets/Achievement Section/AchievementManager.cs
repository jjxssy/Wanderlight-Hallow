using UnityEngine;
using System.Collections.Generic;
using System; 

/// <summary>
/// Central controller for tracking, persisting, and broadcasting achievement progress.
/// 
/// Responsibilities:
/// - Keeps a lookup of all <see cref="Achievement"/>s by ID.
/// - Adds progress and unlocks achievements, firing events for UI/rewards.
/// - Saves/loads per-achievement state with PlayerPrefs.
/// - Exposes a singleton <see cref="Instance"/> for convenience.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance of the manager (survives scene loads).
    /// </summary>
    public static AchievementManager Instance { get; private set; }

    [Tooltip("A list of all achievements in the game (configured in the inspector).")]
    [SerializeField] private List<Achievement> achievements;

    /// <summary>
    /// Fast lookup from achievement ID to its <see cref="Achievement"/> asset.
    /// </summary>
    private readonly Dictionary<string, Achievement> achievementDictionary = new Dictionary<string, Achievement>();

    /// <summary>
    /// Fired when an achievement is unlocked.
    /// </summary>
    public static event Action<Achievement> OnAchievementUnlocked;

    /// <summary>
    /// Fired when progress on an achievement changes (but it may not be unlocked yet).
    /// </summary>
    public static event Action<Achievement> OnProgressUpdated;

    /// <summary>
    /// Establishes the singleton, persists across scenes, and initializes achievements.
    /// </summary>
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

        // NOTE: This line deletes save slot 0 every time the manager awakens.
        // Keep only if this is intentional for testing.
        SaveSystem.DeletePlayer(0);
    }

    /// <summary>
    /// Resets in-memory state, builds the ID lookup, and loads saved progress for each achievement.
    /// </summary>
    private void InitializeAchievements()
    {
        achievementDictionary.Clear();

        foreach (Achievement achievement in achievements)
        {
            if (achievement == null) continue;

            // start clean in memory
            achievement.SetUnlocked(false);
            achievement.SetCurrentProgress(0);

            string id = achievement.GetId();
            if (!string.IsNullOrEmpty(id))
            {
                if (!achievementDictionary.ContainsKey(id))
                {
                    achievementDictionary.Add(id, achievement);
                }
                else
                {
                    Debug.LogWarning($"Duplicate Achievement ID detected: '{id}'. Only the first will be used.");
                }
            }
            else
            {
                Debug.LogWarning("Achievement with empty ID found in list.");
            }

            // load persisted progress/unlocked state
            LoadAchievementProgress(achievement);
        }

        Debug.Log("Achievement Manager Initialized.");
    }

    /// <summary>
    /// Increases progress for the achievement with the given ID and unlocks it if the goal is reached.
    /// </summary>
    /// <param name="achievementId">The unique ID of the achievement to update.</param>
    /// <param name="amount">How much progress to add.</param>
    public void AddProgress(string achievementId, int amount)
    {
        if (string.IsNullOrEmpty(achievementId))
        {
            Debug.LogWarning("AddProgress called with null/empty achievementId.");
            return;
        }

        if (achievementDictionary.TryGetValue(achievementId, out Achievement achievement) && achievement != null)
        {
            if (achievement.GetIsUnlocked()) return;

            int newProgress = Mathf.Max(0, achievement.GetCurrentProgress() + amount);
            achievement.SetCurrentProgress(newProgress);

            OnProgressUpdated?.Invoke(achievement);

            if (achievement.GetCurrentProgress() >= achievement.GetGoal())
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
    /// Marks an achievement as unlocked, clamps progress to goal, and notifies listeners.
    /// </summary>
    private void UnlockAchievement(Achievement achievement)
    {
        if (achievement == null || achievement.GetIsUnlocked()) return;

        achievement.SetUnlocked(true);
        achievement.SetCurrentProgress(achievement.GetGoal());

        Debug.Log($"Achievement Unlocked: {achievement.GetTitle()}");

        OnAchievementUnlocked?.Invoke(achievement);
        SaveAchievementProgress(achievement);
    }

    // ---------------------- Saving and Loading ----------------------

    /// <summary>
    /// Writes the current progress and unlocked flag for a single achievement to PlayerPrefs.
    /// </summary>
    private void SaveAchievementProgress(Achievement achievement)
    {
        if (achievement == null) return;

        string id = achievement.GetId();
        if (string.IsNullOrEmpty(id)) return;

        string progressKey = $"Achievement_{id}_Progress";
        string unlockedKey = $"Achievement_{id}_Unlocked";

        PlayerPrefs.SetInt(progressKey, achievement.GetCurrentProgress());
        PlayerPrefs.SetInt(unlockedKey, achievement.GetIsUnlocked() ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the saved progress/unlocked state for a single achievement (if present).
    /// </summary>
    private void LoadAchievementProgress(Achievement achievement)
    {
        if (achievement == null) return;

        string id = achievement.GetId();
        if (string.IsNullOrEmpty(id)) return;

        string progressKey = $"Achievement_{id}_Progress";
        string unlockedKey = $"Achievement_{id}_Unlocked";

        if (PlayerPrefs.HasKey(progressKey))
        {
            achievement.SetCurrentProgress(PlayerPrefs.GetInt(progressKey));
        }

        if (PlayerPrefs.HasKey(unlockedKey))
        {
            achievement.SetUnlocked(PlayerPrefs.GetInt(unlockedKey) == 1);
        }
    }

    /// <summary>
    /// Resets a single achievement's progress/unlocked state and persists the change.
    /// </summary>
    public void ResetAchievement(string achievementId)
    {
        if (achievementDictionary.TryGetValue(achievementId, out Achievement achievement) && achievement != null)
        {
            achievement.SetCurrentProgress(0);
            achievement.SetUnlocked(false);

            SaveAchievementProgress(achievement);
            OnProgressUpdated?.Invoke(achievement);

            Debug.Log($"Achievement '{achievement.GetTitle()}' has been reset.");
        }
        else
        {
            Debug.LogWarning($"Could not reset achievement. ID '{achievementId}' not found!");
        }
    }

    /// <summary>
    /// Resets all achievements (progress to 0, unlocked to false) and persists each.
    /// </summary>
    public void ResetAllAchievements()
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement == null) continue;

            achievement.SetCurrentProgress(0);
            achievement.SetUnlocked(false);

            SaveAchievementProgress(achievement);
            OnProgressUpdated?.Invoke(achievement);
        }

        Debug.Log("All achievements have been reset.");
    }

    /// <summary>
    /// Returns the list assigned in the inspector (not a copy).
    /// </summary>
    public List<Achievement> GetAchievements()
    {
        return achievements;
    }
}
