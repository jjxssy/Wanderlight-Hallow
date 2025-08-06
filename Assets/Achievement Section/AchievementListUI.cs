using UnityEngine;
using System.Collections.Generic;

public class AchievementListUI : MonoBehaviour
{
    [Tooltip("The prefab for a single achievement UI item.")]
    [SerializeField] private GameObject achievementItemPrefab;

    [Tooltip("The parent object where achievement items will be spawned (the 'Content' object of a Scroll View).")]
    [SerializeField] private Transform contentParent;

    void Start()
    {
        if (achievementItemPrefab == null || contentParent == null)
        {
            Debug.LogError("Setup the AchievementListUI component in the Inspector!");
            return;
        }

        PopulateList();
    }

    private void PopulateList()
    { 
        List<Achievement> allAchievements = AchievementManager.Instance.GetAchievements();
        foreach (Achievement achievement in allAchievements)
        {
            GameObject newItem = Instantiate(achievementItemPrefab, contentParent);
            AchievementUIItem uiItem = newItem.GetComponent<AchievementUIItem>();

            if (uiItem != null)
            {
                uiItem.Setup(achievement);
            }
        }
    }
}