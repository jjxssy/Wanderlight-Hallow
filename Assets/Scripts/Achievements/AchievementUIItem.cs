using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUIItem : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Slider progressBar;

    private Achievement targetAchievement;
    public void Setup(Achievement achievement)
    {
        targetAchievement = achievement;
        UpdateUI();
        AchievementManager.OnProgressUpdated += OnProgressUpdated;
        AchievementManager.OnAchievementUnlocked += OnAchievementUnlocked;
    }

    private void OnDestroy()
    {
        AchievementManager.OnProgressUpdated -= OnProgressUpdated;
        AchievementManager.OnAchievementUnlocked -= OnAchievementUnlocked;
    }

    // Event Handlers
    private void OnProgressUpdated(Achievement achievement)
    {
        if (achievement == targetAchievement) UpdateUI();
    }

    private void OnAchievementUnlocked(Achievement achievement)
    {
        if (achievement == targetAchievement) UpdateUI();
    }

    private void UpdateUI()
    {
        if (targetAchievement == null) return;

        titleText.text = targetAchievement.title;
        descriptionText.text = targetAchievement.description;

        if (targetAchievement.isUnlocked)
        {
            iconImage.sprite = targetAchievement.unlockedIcon;
            progressBar.value = progressBar.maxValue;
            progressText.text = "Completed!";
        }
        else
        {
            iconImage.sprite = targetAchievement.lockedIcon;
            progressBar.maxValue = targetAchievement.goal;
            progressBar.value = targetAchievement.currentProgress;
            progressText.text = $"{targetAchievement.currentProgress} / {targetAchievement.goal}";
        }
    }
}