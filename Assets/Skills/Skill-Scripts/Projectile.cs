using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damageAmount;
    private float pushbackForce; // The projectile now carries a damage value

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f);
    }

    // Update Initialize to accept a damage value
    public void Initialize(Vector2 direction, float speed, int damage, float force, bool isPerpendicular)
    {
        this.damageAmount = damage;
        this.pushbackForce = force;
        rb.linearVelocity = direction * speed;

        // --- ROTATION LOGIC UPDATE ---
        // Calculate the base angle to point forward
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // If the skill is meant to be perpendicular, add 90 degrees
        if (isPerpendicular)
        {
            angle += 90f;
        }

        // Apply the final calculated rotation
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ... (the existing IDamageable logic)
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }
        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
        if (otherRb != null && pushbackForce > 0)
        {
            Vector2 pushDirection = (other.transform.position - transform.position).normalized;
            otherRb.AddForce(pushDirection * pushbackForce, ForceMode2D.Impulse);
        }

        if (otherRb != null)
        {
            Destroy(gameObject);
        }
    }
}