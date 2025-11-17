using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro;

/// <summary>
/// Handles player health, stats, damage, and death behavior.
/// </summary>
public class PlayerStats : MonoBehaviour, IDamageable
{
    public static PlayerStats instance;

    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float invincibilityFrameDuration = 0.5f;

    [Header("Core Stats")]
    [SerializeField] private int strength = 5;
    [SerializeField] private int defense = 0;
    [SerializeField] private int maxMana = 10;
    [SerializeField] private int currentMana = 10;
    [SerializeField] private float speed = 3f;

    [Header("StatsUI")]
    [SerializeField] private TextMeshProUGUI maxHealthText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Events")]
    [SerializeField] private UnityEvent OnDied;
    [SerializeField] private UnityEvent OnDamaged;

    [SerializeField] private Color damageFlashColor = Color.red;
    private Color originalColor;

    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    /// <summary>
    /// Sets up the singleton instance and caches the SpriteRenderer/color.
    /// </summary>
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    /// <summary>
    /// Initializes health for new games and configures the health UI.
    /// </summary>
    private void Start()
    {
        if (PlayerPrefs.GetInt("LoadIndex", 0) == 0)
        {
            currentHealth = maxHealth;
        }
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    /// <summary>
    /// Updates UI every frame and clamps current health to max.
    /// </summary>
    private void Update()
    {
        UpdateUI();
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    /// <summary>
    /// Synchronizes the on-screen stats labels and health slider.
    /// </summary>
    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        if (maxHealthText != null) maxHealthText.text = "" + maxHealth;
        if (defenseText != null) defenseText.text = "" + defense;
        if (strengthText != null) strengthText.text = "" + strength;
        if (speedText != null) speedText.text = "" + speed;
    }

    // === Getters and Setters ===

    public int GetMaxHealth() { return maxHealth; }
    public void SetMaxHealth(int value) { maxHealth = value; }

    public int GetCurrentHealth() { return currentHealth; }
    public void SetCurrentHealth(int value) { currentHealth = Mathf.Clamp(value, 0, maxHealth); }

    public int GetStrength() { return strength; }
    public void SetStrength(int value) { strength = value; }

    public int GetDefense() { return defense; }
    public void SetDefense(int value) { defense = value; }

    public int GetMaxMana() { return maxMana; }
    public void SetMaxMana(int value) { maxMana = value; }

    public int GetCurrentMana() { return currentMana; }
    public void SetCurrentMana(int value) { currentMana = Mathf.Clamp(value, 0, maxMana); }

    public float GetSpeed() { return speed; }
    public void SetSpeed(float value) { speed = value; }

    /// <summary>
    /// Applies damage with defense mitigation, triggers hit feedback and death if health reaches zero.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        float damageMultiplier = 6f / (6f + defense);
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(damage * damageMultiplier));
        SetCurrentHealth(currentHealth - finalDamage);

        OnDamaged?.Invoke();
        StartCoroutine(DamageFlash());
        StartCoroutine(InvincibilityFrames());
        if (currentHealth <= 0)
        {
            PlayerDied();
        }
    }

    /// <summary>Heals the player by a given amount (clamped to max health).</summary>
    public void Heal(int amount)
    {
        SetCurrentHealth(currentHealth + amount);
    }

    /// <summary>Restores mana by a given amount (clamped to max mana).</summary>
    public void RestoreMana(int amount)
    {
        SetCurrentMana(currentMana + amount);
    }

    /// <summary>Increases strength by a given value.</summary>
    public void AddStrength(int value)
    {
        SetStrength(strength + value);
    }

    /// <summary>Increases defense by a given value.</summary>
    public void AddDefense(int value)
    {
        SetDefense(defense + value);
    }

    /// <summary>
    /// Handles death: events, stat tracking, animation, disabling movement/collider, and showing the death UI.
    /// </summary>
    private void PlayerDied()
    {
        OnDied?.Invoke();
        StatisticsManager.Increase("deathCount");

        Animator anim = GetComponent<Animator>();
        if (anim != null) { anim.SetTrigger("Die"); }

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) { movement.enabled = false; }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) { col.enabled = false; }

        // Updated to use the accessor instead of a public instance field.
        var dm = DeathManager.GetInstance();
        if (dm != null)
        {
            dm.GameOver();
        }
    }

    /// <summary>Briefly flashes the sprite to indicate damage.</summary>
    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    /// <summary>Grants temporary invincibility after taking damage.</summary>
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityFrameDuration);
        isInvincible = false;
    }
}
