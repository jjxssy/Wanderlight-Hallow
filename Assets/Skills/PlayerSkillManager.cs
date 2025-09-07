using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public event System.Action OnEquippedSkillsChanged;

    [Header("Skill Data")]
    [SerializeField] private List<Skill> unlockedSkills = new List<Skill>(); // All skills the player has learned
    [SerializeField] private Skill[] equippedSkills = new Skill[6];          // The 6 skills in the skill bar

    [Header("Dependencies")]
    [SerializeField] private KeyBindingsManager keyBindingsManager;
    [SerializeField] private GameObject skillMenuPanel; // The UI panel for assigning skills
    [SerializeField] private SkillSlotUI[] skillSlotsUI; // The 6 UI slots from the skill bar

    private Dictionary<Skill, float> cooldownTimers = new Dictionary<Skill, float>();
    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
        if (skillMenuPanel != null)
        {
            skillMenuPanel.SetActive(false);
        }

        foreach (Skill skill in unlockedSkills)
        {
            if (!cooldownTimers.ContainsKey(skill))
            {
                cooldownTimers.Add(skill, 0f);
            }
        }

        UpdateSkillBarUI();
    }
    


    void Update()
    {
        HandleSkillActivation();
        HandleSkillMenuToggle();

    }
    private void OnEnable()
    {
        // Subscribe to the event when this object becomes active
        KeyBindingsManager.OnKeyBindingsChanged += UpdateSkillBarUI;
    }

    private void OnDisable()
    {
        // Unsubscribe when this object is disabled to prevent errors
        KeyBindingsManager.OnKeyBindingsChanged -= UpdateSkillBarUI;
    }


    private void HandleSkillActivation()
    {
        // Loop through the 6 skill slots
        for (int i = 0; i < equippedSkills.Length; i++)
        {
            if (equippedSkills[i] == null) continue;

            // Get the key for the current slot from the KeyBindingsManager
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

        // Open/close the skill menu
        if (Input.GetKeyDown(keyBindingsManager.GetKey("SKILLS")))
        {
            if (skillMenuPanel.activeSelf)
            {
                skillMenuPanel.SetActive(!skillMenuPanel.activeSelf);
                PlayerPrefs.SetInt("MenusOpen", 0);
            }
            else if (PlayerPrefs.GetInt("MenusOpen", 0) == 0)
            {
                skillMenuPanel.SetActive(!skillMenuPanel.activeSelf);
                PlayerPrefs.SetInt("MenusOpen", 1);
            }
        }
    }

    private void TryActivateSkill(Skill skill, int slotIndex)
    {
        if (!cooldownTimers.ContainsKey(skill) || cooldownTimers[skill] <= Time.time)
        {
            if (animator != null && !string.IsNullOrEmpty(skill.animationTrigger))
            {
                animator.SetTrigger(skill.animationTrigger);
            }

            skill.Activate(gameObject);

            cooldownTimers[skill] = Time.time + skill.cooldown;

            // Tell the UI slot to start its cooldown visual
            if (slotIndex < skillSlotsUI.Length)
            {
                skillSlotsUI[slotIndex].StartCooldown();
            }
        }
        else
        {
            Debug.Log(skill.skillName + " is on cooldown!");
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
            Debug.Log("Learned new skill: " + newSkill.skillName);
        }
    }

    public void EquipSkill(int slotIndex, Skill skillToEquip)
    {
        if (slotIndex < 0 || slotIndex >= equippedSkills.Length) return;

        if (unlockedSkills.Contains(skillToEquip))
        {

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
    }

    private void UpdateSkillBarUI()
    {
        for (int i = 0; i < skillSlotsUI.Length; i++)
        {
            string actionName = "SKILL " + (i + 1);
            KeyCode key = keyBindingsManager.GetKey(actionName);

            if (i < equippedSkills.Length && equippedSkills[i] != null)
            {
                skillSlotsUI[i].SetSkill(equippedSkills[i], key);
            }
            else
            {
                skillSlotsUI[i].SetSkill(null, key);
            }
        }
    }


    public void ApplyTimedBuff(BuffSkill skill, PlayerStats stats)
    {
        StartCoroutine(TimedBuffCoroutine(skill, stats));
    }

    private IEnumerator TimedBuffCoroutine(BuffSkill skill, PlayerStats stats)
    {
        switch (skill.statToBuff)
        {
            case StatType.Strength: stats.AddStrength(skill.buffValue); break;
            case StatType.Defense: stats.AddDefense(skill.buffValue); break;
        }
        yield return new WaitForSeconds(skill.duration);
        switch (skill.statToBuff)
        {
            case StatType.Strength: stats.AddStrength(-skill.buffValue); break;
            case StatType.Defense: stats.AddDefense(-skill.buffValue); break;
        }
    }
    public void ActivateShield(ShieldSkill skill)
    {
        StartCoroutine(ShieldRoutine(skill));
    }

    private IEnumerator ShieldRoutine(ShieldSkill skill)
    {
        GameObject shieldInstance = Instantiate(skill.shieldPrefab, transform.position + new Vector3(0f,2.5f,0f), Quaternion.identity, transform);

        Debug.Log("Shields up for " + skill.duration + " seconds!");
        yield return new WaitForSeconds(skill.duration);

        Debug.Log("Shields down!");
        if (shieldInstance != null)
        {
            Destroy(shieldInstance);
        }
    }
    public List<Skill> GetUnlockedSkills()
    {
        return unlockedSkills;
    }
    public Skill[] GetEquipedSkills()
    {
        return equippedSkills;
    }
}