using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int health;
    [SerializeField] private Slider slider;

    [Header("Events")]
    [SerializeField] private UnityEvent OnDied;
    [SerializeField] private UnityEvent OnDamaged;

    private void Start()
    {
        health = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = health;
    }

    public void takeDamage(int damage)
    {
        health -= damage;
        slider.value = health;
        OnDamaged?.Invoke();

        if (health <= 0)
        {
            PlayerDied();
        }
    }

    private void PlayerDied(){
        OnDied?.Invoke(); // Trigger death UI, sounds, etc.
        
        // Trigger death animation
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Die");

        // Disable gameplay control
        GetComponent<PlayerMovement>().enabled = false;

        // Disable collider or physics if needed
        GetComponent<Collider2D>().enabled = false;

        // Trigger Game Over UI
        DeathManager.instance.GameOver();

    }
}
