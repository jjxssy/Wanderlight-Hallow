using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Handles player health, stats, damage, and death behavior.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float invincibilityFrameDuration = 0.5f;

    [Header("Core Stats")]
    [SerializeField] private int strength = 5;
    [SerializeField] private int defense = 2;
    [SerializeField] private int maxMana = 10;
    [SerializeField] private int currentMana = 10;
    [SerializeField] private float speed = 3f;

    [Header("Events")]
    [SerializeField] private UnityEvent OnDied;
    [SerializeField] private UnityEvent OnDamaged;

    [SerializeField] private Color damageFlashColor = Color.red;
    private Color originalColor;

    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
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
    private void Update()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
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


    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }
        int finalDamage = Mathf.Max(damage - defense, 1);
        SetCurrentHealth(currentHealth - finalDamage);

        OnDamaged?.Invoke();
        StartCoroutine(DamageFlash());
        StartCoroutine(InvincibilityFrames());
        if (currentHealth <= 0)
        {
            PlayerDied();
        }
    }

    public void Heal(int amount)
    {
        SetCurrentHealth(currentHealth + amount);
    }

    public void RestoreMana(int amount)
    {
        SetCurrentMana(currentMana + amount);
    }

    public void AddStrength(int value)
    {
        SetStrength(strength + value);
    }

    public void AddDefense(int value)
    {
        SetDefense(defense + value);
    }

    private void PlayerDied()
    {
        OnDied?.Invoke();

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (DeathManager.instance != null)
        {
            DeathManager.instance.GameOver();
        }
    }
    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityFrameDuration);
        isInvincible = false;
    }
}
