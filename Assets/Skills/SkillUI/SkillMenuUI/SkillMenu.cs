using UnityEngine;

public class SkillMenu : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerSkillManager playerSkillManager;
    [SerializeField] private GameObject draggableSkillIconPrefab; 
    [SerializeField] private Transform unlockedSkillsContainer;

    [SerializeField] private SkillDescriptionUI descriptionPanel;

    private void OnEnable()
    {
        PopulateUnlockedSkills();
    }

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
    public SkillDescriptionUI GetDescriptionPannel()
    {
        return descriptionPanel;
    }
}