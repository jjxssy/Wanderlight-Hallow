using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Builds the achievements list UI at runtime by instantiating one
/// <see cref="AchievementUIItem"/> prefab per <see cref="Achievement"/>
/// managed by <see cref="AchievementManager"/> and parenting it under
/// a scrollable content container.
/// </summary>
public class AchievementListUI : MonoBehaviour
{
    [Tooltip("The prefab for a single achievement UI item.")]
    [SerializeField] private GameObject achievementItemPrefab;

    [Tooltip("The parent object where achievement items will be spawned (the 'Content' object of a Scroll View).")]
    [SerializeField] private Transform contentParent;

    /// <summary>
    /// Validates inspector references and populates the list on scene start.
    /// </summary>
    private void Start()
    {
        if (achievementItemPrefab == null || contentParent == null)
        {
            Debug.LogError("Setup the AchievementListUI component in the Inspector!");
            return;
        }

        PopulateList();
    }

    /// <summary>
    /// Instantiates one UI item per achievement and initializes it with data.
    /// </summary>
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
