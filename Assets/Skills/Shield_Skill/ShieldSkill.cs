using UnityEngine;

/// <summary>
/// Creates a temporary shield via the player's SkillManager, and reports a short UI summary.
/// Uses private serialized fields with Java-style getters/setters.
/// </summary>
[CreateAssetMenu(fileName = "New Shield Skill", menuName = "Skills/Shield Skill")]
public class ShieldSkill : Skill
{
    [Header("Shield Settings")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private GameObject shieldPrefab;

    // --- Java-style getters/setters (no public fields/properties) ---
    public float GetDuration() { return duration; }
    public void SetDuration(float value) { duration = value; }

    public GameObject GetShieldPrefab() { return shieldPrefab; }
    public void SetShieldPrefab(GameObject value) { shieldPrefab = value; }

    /// <summary>
    /// Requests the PlayerSkillManager to activate the shield effect.
    /// </summary>
    public override void Activate(GameObject user)
    {
        var skillManager = user.GetComponent<PlayerSkillManager>();
        if (skillManager != null)
        {
            skillManager.ActivateShield(this);
        }
    }

    /// <summary>
    /// Returns a concise description of this shield's duration.
    /// </summary>
    public override string GetStatDetails()
    {
        return $"Duration: {GetDuration()}s";
    }
}
