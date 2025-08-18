using UnityEngine;

// Enum to easily select which stat to modify in the Inspector
public enum StatType { Health, Mana, Strength, Defense }

[CreateAssetMenu(fileName = "New Buff Skill", menuName = "Skills/Buff Skill")]
public class BuffSkill : Skill
{
    [Header("Buff Settings")]
    public StatType statToBuff;
    public int buffValue;
    public float duration; 

    public override void Activate(GameObject user)
    {
        PlayerStats stats = user.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("BuffSkill Error: User does not have a PlayerStats component!");
            return;
        }

        //If duration is 0, it's an instant effect like healing
        if (duration <= 0)
        {
            ApplyInstantEffect(stats);
        }
        else
        {
            //For timed buffs, we need a MonoBehaviour to run a coroutine.
            PlayerSkillManager skillManager = user.GetComponent<PlayerSkillManager>();
            if (skillManager != null)
            {
                skillManager.ApplyTimedBuff(this, stats);
            }
        }
    }

    private void ApplyInstantEffect(PlayerStats stats)
    {
        switch (statToBuff)
        {
            case StatType.Health:
                stats.Heal(buffValue);
                Debug.Log("Healed for " + buffValue);
                break;
            case StatType.Mana:
                stats.RestoreMana(buffValue);
                Debug.Log("Restored " + buffValue + " mana");
                break;
            default:
                Debug.LogWarning("Instant effect for " + statToBuff + " is not supported. Use a duration > 0.");
                break;
        }
    }
}