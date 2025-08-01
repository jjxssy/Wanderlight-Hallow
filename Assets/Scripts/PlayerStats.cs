using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Handles player health, stats, damage, and death behavior.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private Slider healthSlider;

    [Header("Core Stats")]
    [SerializeField] private int strength = 5;
    [SerializeField] private int defense = 2;
    [SerializeField] private int maxMana = 10;
    [SerializeField] private int currentMana = 10;

    [Header("Events")]
    [SerializeField] private UnityEvent OnDied;
    [SerializeField] private UnityEvent OnDamaged;

    // === Properties ===
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    public int Strength { get => strength; set => strength = value; }
    public int Defense { get => defense; set => defense = value; }

    public int MaxMana { get => maxMana; set => maxMana = value; }
    public int CurrentMana
    {
        get => currentMana;
        set => currentMana = Mathf.Clamp(value, 0, maxMana);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    /// <summary>
    /// Applies damage after subtracting defense. Minimum 1 if incoming > 0.
    /// </summary>
    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - defense, 1);
        CurrentHealth -= finalDamage;

        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
        }

        OnDamaged?.Invoke();

        if (CurrentHealth <= 0)
        {
            PlayerDied();
        }
    }

    /// <summary>
    /// Heals the player.
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (healthSlider != null)
            healthSlider.value = CurrentHealth;
    }

    /// <summary>
    /// Restores mana.
    /// </summary>
    public void RestoreMana(int amount)
    {
        CurrentMana += amount;
    }

    /// <summary>
    /// Boosts strength temporarily or permanently.
    /// </summary>
    public void AddStrength(int value) { Strength += value; }

    /// <summary>
    /// Boosts defense temporarily or permanently.
    /// </summary>
    public void AddDefense(int value) { Defense += value; }

    /// <summary>
    /// Called when player health reaches 0.
    /// </summary>
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
}
