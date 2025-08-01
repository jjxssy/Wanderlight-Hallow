using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player stats such as health, speed, strength, and defense.
/// Handles damage, healing, and stat boosts.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private Slider healthSlider;

    [Header("Movement & Stats")]
    [SerializeField] private float baseSpeed = 15f;
    private float currentSpeed;

    [Header("Combat Stats")]
    [SerializeField] private int strength = 5;
    [SerializeField] private int defense = 2;

    // === Properties ===

    /// <summary>
    /// Maximum health value for the player.
    /// </summary>
    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    /// <summary>
    /// Current health value. Clamped between 0 and MaxHealth.
    /// </summary>
    public int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    /// <summary>
    /// Base movement speed of the player.
    /// </summary>
    public float BaseSpeed
    {
        get => baseSpeed;
        set => baseSpeed = value;
    }

    /// <summary>
    /// Current movement speed of the player.
    /// </summary>
    public float CurrentSpeed
    {
        get => currentSpeed;
        set => currentSpeed = value;
    }

    /// <summary>
    /// Player's attack power.
    /// </summary>
    public int Strength
    {
        get => strength;
        set => strength = value;
    }

    /// <summary>
    /// Player's defense value which reduces incoming damage.
    /// </summary>
    public int Defense
    {
        get => defense;
        set => defense = value;
    }

    /// <summary>
    /// Initializes health and movement speed.
    /// </summary>
    private void Start()
    {
        CurrentHealth = MaxHealth;
        CurrentSpeed = BaseSpeed;

        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = CurrentHealth;
        }
    }

    /// <summary>
    /// Applies damage to the player after subtracting defense.
    /// Minimum of 1 damage is always taken if damage is greater than 0.
    /// </summary>
    /// <param name="damage">Raw incoming damage from source</param>
    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - Defense, 1);
        CurrentHealth -= finalDamage;

        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
        }

        if (CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Heals the player by a fixed amount.
    /// </summary>
    /// <param name="amount">Healing value to apply</param>
    public void Heal(int amount)
    {
        CurrentHealth += amount;

        if (healthSlider != null)
            healthSlider.value = CurrentHealth;
    }

    /// <summary>
    /// Temporarily applies stat boosts like strength and defense.
    /// </summary>
    /// <param name="str">Strength boost</param>
    /// <param name="def">Defense boost</param>
    public void ApplyStats(int str, int def)
    {
        Strength += str;
        Defense += def;
    }

    /// <summary>
    /// Removes stat boosts previously applied.
    /// </summary>
    /// <param name="str">Strength to remove</param>
    /// <param name="def">Defense to remove</param>
    public void RemoveStats(int str, int def)
    {
        Strength -= str;
        Defense -= def;
    }
}
