using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destroy the projectile after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);
    }

    public void Initialize(Vector2 direction, float speed)
    {
        // Using linearVelocity as you requested in your saved info
        rb.linearVelocity = direction * speed;

        // Rotate the projectile to face the direction it's moving
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it hit an enemy (we'll assume enemies have an "Enemy" tag)
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit an enemy!");
            // Here you would deal damage to the enemy
            // other.GetComponent<EnemyHealth>().TakeDamage(10);

            Destroy(gameObject); // Destroy the projectile on impact
        }
    }
}