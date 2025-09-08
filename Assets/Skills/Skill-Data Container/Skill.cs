using UnityEngine;

/// <summary>
/// Base ScriptableObject for a skill with private serialized data.
/// Exposes Java-style getter/setter methods for inspector-driven fields,
/// and virtual behavior methods for stat details and activation.
/// </summary>
[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Base Skill")]
public class Skill : ScriptableObject
{
    [Header("Skill Info")]
    /// <summary>Display name of the skill.</summary>
    [SerializeField] private string skillName;
    /// <summary>Icon shown in UI for this skill.</summary>
    [SerializeField] private Sprite skillIcon;
    /// <summary>Cooldown duration in seconds between activations.</summary>
    [SerializeField] private float cooldown = 1f;

    [Header("Effects & Animation")]
    /// <summary>Optional VFX prefab spawned when the skill activates.</summary>
    [SerializeField] private GameObject vfxPrefab;
    /// <summary>Animator trigger name to fire on activation (optional).</summary>
    [SerializeField] private string animationTrigger;

    [Header("UI & Description")]
    /// <summary>Long-form description used by UI.</summary>
    [TextArea(3, 5)]
    [SerializeField] private string description;
    /// <summary>Optional stat-type icon for UI (e.g., strength/defense).</summary>
    [SerializeField] private Sprite statIcon;

    // ---- Java-style getters/setters (no public properties) ----

    /// <summary>Returns the skill's display name.</summary>
    public string GetSkillName() { return skillName; }
    /// <summary>Sets the skill's display name.</summary>
    public void SetSkillName(string value) { skillName = value; }

    /// <summary>Returns the skill's icon sprite.</summary>
    public Sprite GetSkillIcon() { return skillIcon; }
    /// <summary>Sets the skill's icon sprite.</summary>
    public void SetSkillIcon(Sprite value) { skillIcon = value; }

    /// <summary>Returns the cooldown in seconds.</summary>
    public float GetCooldown() { return cooldown; }
    /// <summary>Sets the cooldown in seconds.</summary>
    public void SetCooldown(float value) { cooldown = value; }

    /// <summary>Returns the VFX prefab to spawn on activation.</summary>
    public GameObject GetVfxPrefab() { return vfxPrefab; }
    /// <summary>Sets the VFX prefab to spawn on activation.</summary>
    public void SetVfxPrefab(GameObject value) { vfxPrefab = value; }

    /// <summary>Returns the animator trigger used on activation.</summary>
    public string GetAnimationTrigger() { return animationTrigger; }
    /// <summary>Sets the animator trigger used on activation.</summary>
    public void SetAnimationTrigger(string value) { animationTrigger = value; }

    /// <summary>Returns the long-form UI description.</summary>
    public string GetDescription() { return description; }
    /// <summary>Sets the long-form UI description.</summary>
    public void SetDescription(string value) { description = value; }

    /// <summary>Returns the stat-type icon used in UI.</summary>
    public Sprite GetStatIcon() { return statIcon; }
    /// <summary>Sets the stat-type icon used in UI.</summary>
    public void SetStatIcon(Sprite value) { statIcon = value; }

    // ---- Behavior ----

    /// <summary>
    /// Returns a short, UI-friendly string describing stat effects.
    /// Override in derived skills to provide details.
    /// </summary>
    public virtual string GetStatDetails()
    {
        return "";
    }

    /// <summary>
    /// Executes the skill's behavior. Override in derived classes.
    /// </summary>
    /// <param name="user">GameObject that activated the skill.</param>
    public virtual void Activate(GameObject user)
    {
        Debug.Log(skillName + " was activated by " + user.name);
    }
}
