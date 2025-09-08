using UnityEngine;

/// <summary>
/// ScriptableObject that defines a single achievement:
/// id, title/description, icons, goal, and tracked progress/unlocked state.
/// All fields are private; access is via simple getters and minimal setters.
/// </summary>
[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievements/Achievement")]
public class Achievement : ScriptableObject
{
    [Header("Identification")]
    [Tooltip("A unique ID for this achievement. E.g., 'kill_10_goblins'.")]
    [SerializeField] private string id;

    [Header("Display")]
    [Tooltip("The title displayed to the player.")]
    [SerializeField] private string title;

    [Tooltip("What the player needs to do.")]
    [TextArea]
    [SerializeField] private string description;

    [Tooltip("Icon to show while the achievement is locked.")]
    [SerializeField] private Sprite lockedIcon;

    [Tooltip("Icon to show after the achievement is unlocked.")]
    [SerializeField] private Sprite unlockedIcon;

    [Header("Progress")]
    [Tooltip("How much progress is required to unlock this achievement.")]
    [Min(1)]
    [SerializeField] private int goal = 1;

    [Tooltip("Whether the achievement has been unlocked.")]
    [SerializeField] private bool isUnlocked;

    [Tooltip("Current progress toward the goal.")]
    [SerializeField] private int currentProgress;

    /// <summary>Returns the unique ID.</summary>
    public string GetId() => id;

    /// <summary>Returns the title.</summary>
    public string GetTitle() => title;

    /// <summary>Returns the description.</summary>
    public string GetDescription() => description;

    /// <summary>Returns the locked-state icon.</summary>
    public Sprite GetLockedIcon() => lockedIcon;

    /// <summary>Returns the unlocked-state icon.</summary>
    public Sprite GetUnlockedIcon() => unlockedIcon;

    /// <summary>Returns the required goal amount.</summary>
    public int GetGoal() => goal;

    /// <summary>Returns whether the achievement is unlocked.</summary>
    public bool GetIsUnlocked() => isUnlocked;

    /// <summary>Returns the current progress amount.</summary>
    public int GetCurrentProgress() => currentProgress;

    /// <summary>Sets unlocked state directly.</summary>
    public void SetUnlocked(bool value) => isUnlocked = value;

    /// <summary>Sets current progress (clamped to 0..goal).</summary>
    public void SetCurrentProgress(int value)
    {
        currentProgress = Mathf.Clamp(value, 0, Mathf.Max(1, goal));
    }

    /// <summary>
    /// Adds progress (default +1). Returns true if this call caused the achievement to unlock.
    /// </summary>
    public bool AddProgress(int amount = 1)
    {
        if (isUnlocked) return false;

        currentProgress = Mathf.Clamp(currentProgress + amount, 0, Mathf.Max(1, goal));
        if (currentProgress >= goal)
        {
            isUnlocked = true;
            return true;
        }
        return false;
    }

    /// <summary>Clears progress and relocks the achievement.</summary>
    public void ResetProgress()
    {
        currentProgress = 0;
        isUnlocked = false;
    }
}
