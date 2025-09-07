using UnityEngine;

[CreateAssetMenu(fileName = "New Shield Skill", menuName = "Skills/Shield Skill")]
public class ShieldSkill : Skill
{
    [Header("Shield Settings")]
    public float duration = 5f;
    public GameObject shieldPrefab; // The VFX prefab we just made

    public override void Activate(GameObject user)
    {
        PlayerSkillManager skillManager = user.GetComponent<PlayerSkillManager>();
        if (skillManager != null)
        {
            skillManager.ActivateShield(this);
        }
    }

    public override string GetStatDetails()
    {
        return $"Duration: {duration}s";
    }
}