using UnityEngine;

/// <summary>
/// Handles damage application when an enemy collides with the player.
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    /// <summary>
    /// On collision with player, apply damage.
    /// </summary>
    /// <param name="collision">Collision event</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}
