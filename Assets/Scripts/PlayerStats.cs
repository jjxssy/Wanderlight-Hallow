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
    [SerializeField] private float speed = 3f;

    [Header("Events")]
    [SerializeField] private UnityEvent OnDied;
    [SerializeField] private UnityEvent OnDamaged;

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

    // === Unity Methods ===

    private void Start()
    {
        currentHealth = maxHealth;
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

    // === Core Logic ===

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - defense, 1);
        SetCurrentHealth(currentHealth - finalDamage);

        OnDamaged?.Invoke();

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
}
