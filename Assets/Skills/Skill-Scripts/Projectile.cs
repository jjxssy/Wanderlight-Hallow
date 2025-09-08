using UnityEngine;

/// <summary>
/// Simple 2D projectile: flies in a direction, optionally rotates perpendicular,
/// applies damage and pushback on trigger, and self-destructs after a timeout.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    /// <summary>Cached Rigidbody2D used to move the projectile.</summary>
    private Rigidbody2D rb;

    /// <summary>Amount of damage dealt on hit.</summary>
    private int damageAmount;

    /// <summary>Impulse force applied to the hit target along travel direction.</summary>
    private float pushbackForce; 

    /// <summary>
    /// Caches the rigidbody and schedules auto-destroy after 5 seconds.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f);
    }

    /// <summary>
    /// Configures and launches the projectile.
    /// </summary>
    /// <param name="direction">Normalized world-space direction to travel.</param>
    /// <param name="speed">Travel speed in units per second.</param>
    /// <param name="damage">Damage to apply on impact.</param>
    /// <param name="force">Pushback impulse to apply on impact.</param>
    /// <param name="isPerpendicular">If true, rotate the sprite by +90° relative to travel.</param>
    public void Initialize(Vector2 direction, float speed, int damage, float force, bool isPerpendicular)
    {
        this.damageAmount = damage;
        this.pushbackForce = force;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (isPerpendicular)
        {
            angle += 90f;
        }

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Applies damage and pushback to valid targets, then destroys the projectile on any Rigidbody2D contact.
    /// </summary>
    /// <param name="other">The collider that the projectile entered.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }

        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
        if (otherRb != null && pushbackForce > 0)
        {
            Vector2 pushDirection = rb.linearVelocity.normalized;
            otherRb.AddForce(pushDirection * pushbackForce, ForceMode2D.Impulse);
        }

        if (otherRb != null)
        {
            Destroy(gameObject);
        }
    }
}
