using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's learned skills, the 6-slot skill bar, input-driven activation
/// (with cooldowns and animation triggers), the skills menu toggle, and notifies
/// listeners whenever the equipped skills layout changes.
/// </summary>
public class PlayerSkillManager : MonoBehaviour
{
    /// <summary>
    /// Event invoked whenever the equipped skills array changes.
    /// Subscribe via <see cref="AddOnEquippedSkillsChangedListener"/> and
    /// unsubscribe via <see cref="RemoveOnEquippedSkillsChangedListener"/>.
    /// </summary>
    private event System.Action OnEquippedSkillsChanged;

    [Header("Skill Data")]
    /// <summary>All skills the player has learned/unlocked.</summary>
    [SerializeField] private List<Skill> unlockedSkills = new List<Skill>();

    /// <summary>The 6 skills currently placed on the player's skill bar.</summary>
    [SerializeField] private Skill[] equippedSkills = new Skill[6];

    [Header("Dependencies")]
    /// <summary>Provides key lookups for skill activation (e.g., "SKILL 1").</summary>
    [SerializeField] private KeyBindingsManager keyBindingsManager;

    /// <summary>Root GameObject for the assign/arrange skills menu.</summary>
    [SerializeField] private GameObject skillMenuPanel;

    /// <summary>References to the UI widgets representing each skill bar slot.</summary>
    [SerializeField] private SkillSlotUI[] skillSlotsUI;

    /// <summary>Tracks next usable time for each skill (world time seconds).</summary>
    private Dictionary<Skill, float> cooldownTimers = new Dictionary<Skill, float>();

    /// <summary>Cached Animator for optional skill animation triggers.</summary>
    private Animator animator;

    // --- Simple getters (kept your names/signatures) ---
    public List<Skill> GetUnlockedSkills() { return unlockedSkills; }
    public Skill[] GetEquipedSkills() { return equippedSkills; }

    // --- Subscribe/unsubscribe for the private event ---
    public void AddOnEquippedSkillsChangedListener(System.Action listener) { OnEquippedSkillsChanged += listener; }
    public void RemoveOnEquippedSkillsChangedListener(System.Action listener) { OnEquippedSkillsChanged -= listener; }

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (skillMenuPanel != null) skillMenuPanel.SetActive(false);

        foreach (Skill skill in unlockedSkills)
            if (!cooldownTimers.ContainsKey(skill)) cooldownTimers.Add(skill, 0f);

        UpdateSkillBarUI();
    }

    private void Update()
    {
        HandleSkillActivation();
        HandleSkillMenuToggle();
    }

    private void OnEnable()
    {
        KeyBindingsManager.OnKeyBindingsChanged += UpdateSkillBarUI;
    }

    private void OnDisable()
    {
        KeyBindingsManager.OnKeyBindingsChanged -= UpdateSkillBarUI;
    }

    private void HandleSkillActivation()
    {
        for (int i = 0; i < equippedSkills.Length; i++)
        {
            if (equippedSkills[i] == null) continue;

            string actionName = "SKILL " + (i + 1);
            KeyCode skillKey = keyBindingsManager.GetKey(actionName);

            if (Input.GetKeyDown(skillKey))
            {
                TryActivateSkill(equippedSkills[i], i);
            }
        }
    }

    private void HandleSkillMenuToggle()
    {
        if (skillMenuPanel == null) return;

        if (Input.GetKeyDown(keyBindingsManager.GetKey("SKILLS")))
        {
            if (skillMenuPanel.activeSelf)
            {
                skillMenuPanel.SetActive(false);
                PlayerPrefs.SetInt("MenusOpen", 0);
            }
            else if (PlayerPrefs.GetInt("MenusOpen", 0) == 0)
            {
                skillMenuPanel.SetActive(true);
                PlayerPrefs.SetInt("MenusOpen", 1);
            }
        }
    }

    private void TryActivateSkill(Skill skill, int slotIndex)
    {
        if (!cooldownTimers.ContainsKey(skill) || cooldownTimers[skill] <= Time.time)
        {
            string trigger = skill.GetAnimationTrigger();
            if (animator != null && !string.IsNullOrEmpty(trigger))
            {
                animator.SetTrigger(trigger);
            }

            skill.Activate(gameObject);

            cooldownTimers[skill] = Time.time + skill.GetCooldown();

            if (slotIndex < skillSlotsUI.Length)
            {
                skillSlotsUI[slotIndex].StartCooldown();
            }
        }
        else
        {
            Debug.Log(skill.GetSkillName() + " is on cooldown!");
        }
    }

    public void LearnSkill(Skill newSkill)
    {
        if (!unlockedSkills.Contains(newSkill))
        {
            unlockedSkills.Add(newSkill);
            if (!cooldownTimers.ContainsKey(newSkill))
            {
                cooldownTimers.Add(newSkill, 0f);
            }
            Debug.Log("Learned new skill: " + newSkill.GetSkillName());
        }
    }

    public void EquipSkill(int slotIndex, Skill skillToEquip)
    {
        if (slotIndex < 0 || slotIndex >= equippedSkills.Length) return;
        if (!unlockedSkills.Contains(skillToEquip)) return;

        // ensure no duplicates
        for (int i = 0; i < equippedSkills.Length; i++)
        {
            if (equippedSkills[i] == skillToEquip)
            {
                equippedSkills[i] = null;
                break;
            }
        }

        equippedSkills[slotIndex] = skillToEquip;

        UpdateSkillBarUI();
        OnEquippedSkillsChanged?.Invoke();
    }

    private void UpdateSkillBarUI()
    {
        for (int i = 0; i < skillSlotsUI.Length; i++)
        {
            string actionName = "SKILL " + (i + 1);
            KeyCode key = keyBindingsManager.GetKey(actionName);

            if (i < equippedSkills.Length && equippedSkills[i] != null)
                skillSlotsUI[i].SetSkill(equippedSkills[i], key);
            else
                skillSlotsUI[i].SetSkill(null, key);
        }
    }

    /// <summary>
    /// Starts a timed buff coroutine that applies a temporary Strength/Defense bonus.
    /// </summary>
    public void ApplyTimedBuff(BuffSkill skill, PlayerStats stats)
    {
        StartCoroutine(TimedBuffCoroutine(skill, stats));
    }

    private IEnumerator TimedBuffCoroutine(BuffSkill skill, PlayerStats stats)
    {
        switch (skill.GetStatToBuff())
        {
            case StatType.Strength: stats.AddStrength(skill.GetBuffValue()); break;
            case StatType.Defense:  stats.AddDefense (skill.GetBuffValue()); break;
        }

        yield return new WaitForSeconds(skill.GetDuration());

        switch (skill.GetStatToBuff())
        {
            case StatType.Strength: stats.AddStrength(-skill.GetBuffValue()); break;
            case StatType.Defense:  stats.AddDefense (-skill.GetBuffValue()); break;
        }
    }

    /// <summary>
    /// Requests a temporary shield activation around the player.
    /// </summary>
    public void ActivateShield(ShieldSkill skill)
    {
        StartCoroutine(ShieldRoutine(skill));
    }

    private IEnumerator ShieldRoutine(ShieldSkill skill)
    {
        GameObject prefab = skill.GetShieldPrefab();
        if (prefab != null)
        {
            GameObject shieldInstance = Instantiate(prefab, transform.position + new Vector3(0f, 2.5f, 0f), Quaternion.identity, transform);

            Debug.Log("Shields up for " + skill.GetDuration() + " seconds!");
            yield return new WaitForSeconds(skill.GetDuration());

            Debug.Log("Shields down!");
            if (shieldInstance != null)
            {
                Destroy(shieldInstance);
            }
        }
        else
        {
            Debug.LogWarning("ShieldSkill has no shield prefab assigned.");
        }
    }
}
