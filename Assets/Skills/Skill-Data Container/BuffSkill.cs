using UnityEngine;

public enum StatType { Health, Mana, Strength, Defense }

/// <summary>
/// A skill that applies an instant or timed buff to the player (heal/mana restore
/// instantly, or temporary Strength/Defense via PlayerSkillManager coroutine).
/// Uses private serialized fields with Java-style getters/setters.
/// </summary>
[CreateAssetMenu(fileName = "New Buff Skill", menuName = "Skills/Buff Skill")]
public class BuffSkill : Skill
{
    [Header("Buff Settings")]
    /// <summary>Which stat this skill affects (Health, Mana, Strength, Defense).</summary>
    [SerializeField] private StatType statToBuff;

    /// <summary>Magnitude of the effect (e.g., +10 Health, +3 Defense).</summary>
    [SerializeField] private int buffValue;

    /// <summary>Duration in seconds. If ≤ 0, the effect is instant (e.g., heal).</summary>
    [SerializeField] private float duration;

    // --- Java-style getters/setters (no public fields/properties) ---

    /// <summary>Returns the stat type this buff affects.</summary>
    public StatType GetStatToBuff() { return statToBuff; }
    /// <summary>Sets the stat type this buff affects.</summary>
    public void SetStatToBuff(StatType value) { statToBuff = value; }

    /// <summary>Returns the buff amount.</summary>
    public int GetBuffValue() { return buffValue; }
    /// <summary>Sets the buff amount.</summary>
    public void SetBuffValue(int value) { buffValue = value; }

    /// <summary>Returns the buff duration (seconds). ≤ 0 means instant.</summary>
    public float GetDuration() { return duration; }
    /// <summary>Sets the buff duration (seconds).</summary>
    public void SetDuration(float value) { duration = value; }

    /// <summary>
    /// Activates the buff: plays optional VFX, applies an instant effect for Health/Mana,
    /// or starts a timed buff via <see cref="PlayerSkillManager"/> for Strength/Defense.
    /// </summary>
    /// <param name="user">GameObject using the skill (usually the player).</param>
    public override void Activate(GameObject user)
    {
        var stats = user.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("BuffSkill Error: User does not have a PlayerStats component!");
            return;
        }

        var vfx = GetVfxPrefab();
        if (vfx != null)
        {
            var vfxObj = Instantiate(vfx, user.transform);
            vfxObj.transform.position = user.transform.position;
            Destroy(vfxObj, 1f);
        }

        // If duration is 0, it's an instant effect like healing
        if (GetDuration() <= 0f)
        {
            ApplyInstantEffect(stats);
        }
        else
        {
            // For timed buffs, we need a MonoBehaviour to run a coroutine.
            var skillManager = user.GetComponent<PlayerSkillManager>();
            if (skillManager != null)
            {
                skillManager.ApplyTimedBuff(this, stats);
            }
        }
    }

    /// <summary>
    /// Applies an immediate effect to health or mana based on <see cref="statToBuff"/>.
    /// </summary>
    /// <param name="stats">Target player stats to modify.</param>
    private void ApplyInstantEffect(PlayerStats stats)
    {
        switch (GetStatToBuff())
        {
            case StatType.Health:
                stats.Heal(GetBuffValue());
                Debug.Log("Healed for " + GetBuffValue());
                break;

            case StatType.Mana:
                stats.RestoreMana(GetBuffValue());
                Debug.Log("Restored " + GetBuffValue() + " mana");
                break;

            default:
                Debug.LogWarning("Instant effect for " + GetStatToBuff() + " is not supported. Use a duration > 0.");
                break;
        }
    }

    /// <summary>
    /// Returns a concise UI string describing the effect (e.g., "Heal: +10 Health for 5s").
    /// </summary>
    public override string GetStatDetails()
    {
        string effect = GetStatToBuff() == StatType.Health ? "Heal" : "Buff";
        string durationText = GetDuration() > 0f ? $" for {GetDuration()}s" : "";
        return $"{effect}: +{GetBuffValue()} {GetStatToBuff()}{durationText}";
    }
}
