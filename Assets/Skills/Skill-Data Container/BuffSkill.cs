using UnityEngine;
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

        if (vfxPrefab != null)
        {
            GameObject vfxObj = Instantiate(vfxPrefab,user.transform);
            vfxObj.transform.position = user.transform.position;
            Destroy(vfxObj, 1);
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

    public override string GetStatDetails()
    {
        string effect = statToBuff == StatType.Health ? "Heal" : "Buff";
        string durationText = duration > 0 ? $" for {duration}s" : "";
        return $"{effect}: +{buffValue} {statToBuff}{durationText}";
    }
}