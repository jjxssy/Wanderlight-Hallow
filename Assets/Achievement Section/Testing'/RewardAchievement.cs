using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Listens for achievement unlock events, grants rewards, and shows a popup.
/// Popups are queued so multiple unlocks don't interrupt each other.
/// </summary>
public class RewardAchievement : MonoBehaviour
{
    [Header("Popup UI")]
    [SerializeField] private GameObject popUpImage;
    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField, Min(0.1f)] private float popupDuration = 1.5f;

    [Header("Rewards")]
    [SerializeField] private Item healthPotion;

    // simple queue so multiple achievements display sequentially
    private readonly Queue<string> _popupQueue = new Queue<string>();
    private float _popupTimer;
    private bool _isShowing;

    private void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += HandleAchievementCompleted;
    }

    private void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= HandleAchievementCompleted;
    }

    /// <summary>
    /// Handles rewards and enqueues a popup message when an achievement is completed.
    /// </summary>
    private void HandleAchievementCompleted(Achievement completedAchievement)
    {
        if (completedAchievement == null) return;

        Debug.Log("Broadcast Received! Achievement completed: " + completedAchievement.GetTitle());

        // Example rewards by ID
        switch (completedAchievement.GetId())
        {
            case "001":
                EnqueuePopup("Pressed X 5 times");
                if (healthPotion != null)
                {
                    InventoryManager.Instance.AddItem(healthPotion);
                }
                break;

            case "002":
                EnqueuePopup("Pressed Y 10 times");
                break;

            default:
                // Generic fallback if you want to auto-message on any unlock:
                EnqueuePopup($"Unlocked: {completedAchievement.GetTitle()}");
                break;
        }
    }

    /// <summary>
    /// Adds a message to the popup queue.
    /// </summary>
    private void EnqueuePopup(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        _popupQueue.Enqueue(message);
    }

    private void Update()
    {
        // If nothing showing and we have items queued, start the next popup
        if (!_isShowing && _popupQueue.Count > 0)
        {
            string next = _popupQueue.Dequeue();
            ShowPopup(next);
        }

        // Countdown the current popup
        if (_isShowing)
        {
            _popupTimer -= Time.deltaTime;
            if (_popupTimer <= 0f)
            {
                HidePopup();
            }
        }
    }

    /// <summary>
    /// Shows the popup with the given text and resets the timer.
    /// </summary>
    private void ShowPopup(string text)
    {
        if (popUpImage != null) popUpImage.SetActive(true);
        if (popUpText != null) popUpText.text = text;

        _popupTimer = popupDuration;
        _isShowing  = true;
    }

    /// <summary>
    /// Hides the popup and allows the next one (if any) to start.
    /// </summary>
    private void HidePopup()
    {
        if (popUpImage != null) popUpImage.SetActive(false);
        _isShowing = false;
    }
}
