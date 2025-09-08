using UnityEngine;

/// <summary>
/// Populates and manages the Skills menu UI: lists unlocked skills as draggable icons
/// and wires them to a description panel for details.
/// </summary>
public class SkillMenu : MonoBehaviour
{
    [Header("Dependencies")]
    /// <summary>Reference to the player's skill manager (source of unlocked skills).</summary>
    [SerializeField] private PlayerSkillManager playerSkillManager;

    /// <summary>Prefab used to render a single draggable skill icon.</summary>
    [SerializeField] private GameObject draggableSkillIconPrefab; 

    /// <summary>Parent transform that will contain instantiated unlocked skill icons.</summary>
    [SerializeField] private Transform unlockedSkillsContainer;

    /// <summary>Panel that shows the selected skill's description and details.</summary>
    [SerializeField] private SkillDescriptionUI descriptionPanel;

    /// <summary>
    /// When the menu opens, rebuild the list of unlocked skills.
    /// </summary>
    private void OnEnable()
    {
        PopulateUnlockedSkills();
    }

    /// <summary>
    /// Clears existing icons and instantiates a draggable icon for each unlocked skill.
    /// </summary>
    private void PopulateUnlockedSkills()
    {
        if (playerSkillManager == null || draggableSkillIconPrefab == null || unlockedSkillsContainer == null)
        {
            Debug.LogError("SkillMenu is missing necessary references!");
            return;
        }

        // Clear out any old icons first
        foreach (Transform child in unlockedSkillsContainer)
        {
            Destroy(child.gameObject);
        }

        // Create a new icon for each skill the player has learned
        foreach (Skill skill in playerSkillManager.GetUnlockedSkills())
        {
            GameObject iconGO = Instantiate(draggableSkillIconPrefab, unlockedSkillsContainer);

            DraggableSkillIcon draggableIcon = iconGO.GetComponent<DraggableSkillIcon>();
            if (draggableIcon != null)
            {
                draggableIcon.Initialize(skill, descriptionPanel);
            }
        }
    }

    /// <summary>
    /// Returns the description panel reference used by skill icons to display details.
    /// </summary>
    public SkillDescriptionUI GetDescriptionPannel()
    {
        return descriptionPanel;
    }
}
