using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Binds one <see cref="Achievement"/> to a row of UI: title, description,
/// icon, progress bar, and progress label. Subscribes to AchievementManager
/// events to update reactively.
/// </summary>
public class AchievementUIItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Slider progressBar;

    /// <summary>The achievement shown by this UI row.</summary>
    private Achievement targetAchievement;

    /// <summary>
    /// Initializes this UI row for a specific achievement and subscribes to updates.
    /// </summary>
    public void Setup(Achievement achievement)
    {
        targetAchievement = achievement;
        UpdateUI();

        // These events are assumed to exist on your AchievementManager.
        AchievementManager.OnProgressUpdated += OnProgressUpdated;
        AchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
    }

    /// <summary>Unsubscribes from events to avoid leaks.</summary>
    private void OnDestroy()
    {
        AchievementManager.OnProgressUpdated -= OnProgressUpdated;
        AchievementManager.OnAchievementUnlocked -= OnAchievementUnlocked;
    }

    // ---- Event handlers ----

    private void OnProgressUpdated(Achievement achievement)
    {
        if (achievement == targetAchievement) UpdateUI();
    }

    private void OnAchievementUnlocked(Achievement achievement)
    {
        if (achievement == targetAchievement) UpdateUI();
    }

    /// <summary>
    /// Refreshes all UI elements from the current achievement state.
    /// Uses getters, since fields on Achievement are private.
    /// </summary>
    private void UpdateUI()
    {
        if (targetAchievement == null) return;

        // Texts
        if (titleText)       titleText.text       = targetAchievement.GetTitle();
        if (descriptionText) descriptionText.text = targetAchievement.GetDescription();

        // Progress bar baseline (max = goal)
        if (progressBar) progressBar.maxValue = targetAchievement.GetGoal();

        if (targetAchievement.GetIsUnlocked())
        {
            if (iconImage)   iconImage.sprite = targetAchievement.GetUnlockedIcon();
            if (progressBar) progressBar.value = progressBar.maxValue;
            if (progressText) progressText.text = "Completed!";
        }
        else
        {
            if (iconImage)   iconImage.sprite = targetAchievement.GetLockedIcon();
            if (progressBar) progressBar.value = targetAchievement.GetCurrentProgress();
            if (progressText) progressText.text =
                $"{targetAchievement.GetCurrentProgress()} / {targetAchievement.GetGoal()}";
        }
    }
}
