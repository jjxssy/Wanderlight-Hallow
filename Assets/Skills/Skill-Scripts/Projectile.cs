using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damageAmount;
    private float pushbackForce; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f);
    }

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

    void OnTriggerEnter2D(Collider2D other)
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