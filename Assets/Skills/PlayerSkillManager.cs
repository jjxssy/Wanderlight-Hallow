using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public List<Skill> skills;
    private Dictionary<Skill, float> cooldownTimers = new Dictionary<Skill, float>();
    public Animator playerAnimator;
    public PlayerStats playerStats;

    void Start()
    {
        //animator = GetComponent<Animator>();
        foreach (Skill skill in skills)
        {
            cooldownTimers[skill] = 0f;
        }

    }

    void Update()
    {
        //lets say there are only 2 skills for now
        if (Input.GetKeyDown(KeyCode.Alpha1) && skills.Count > 0)
        {
            TryActivateSkill(skills[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && skills.Count > 1)
        {
            TryActivateSkill(skills[1]);
        }
        //Can Implement a skill slot system, which will determine which skill will be used when pressed which key.
    }

    private void TryActivateSkill(Skill skill)
    {
        if (cooldownTimers[skill] <= Time.time)
        {
            if (playerAnimator != null && !string.IsNullOrEmpty(skill.animationTrigger))
            {
                playerAnimator.SetTrigger(skill.animationTrigger);
            }

            skill.Activate(gameObject);

            if (skill.vfxPrefab != null)
            {
                Instantiate(skill.vfxPrefab, transform.position, Quaternion.identity);
            }

            cooldownTimers[skill] = Time.time + skill.cooldown;

            Debug.Log(skill.skillName + " is USED");
        }
        else
        {
            Debug.Log(skill.skillName + " is on cooldown");
        }
    }

    // ====Buff Skill Management====

    public void ApplyTimedBuff(BuffSkill skill, PlayerStats stats)
    {
        StartCoroutine(TimedBuffCoroutine(skill, stats));
    }

    private IEnumerator TimedBuffCoroutine(BuffSkill skill, PlayerStats stats)
    {
        //Apply the buff
        switch (skill.statToBuff)
        {
            case StatType.Strength:
                stats.AddStrength(skill.buffValue);
                break;
            case StatType.Defense:
                stats.AddDefense(skill.buffValue);
                break;
        }

        Debug.Log("Applied " + skill.skillName + " for " + skill.duration + " seconds.");

        yield return new WaitForSeconds(skill.duration);

        //Remove the buff by applying the negative value
        switch (skill.statToBuff)
        {
            case StatType.Strength:
                stats.AddStrength(-skill.buffValue);
                break;
            case StatType.Defense:
                stats.AddDefense(-skill.buffValue);
                break;
        }

        Debug.Log(skill.skillName + " has worn off.");
    }
}