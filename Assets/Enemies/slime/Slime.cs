using System.Collections;
using UnityEngine;
/// <summary>
/// Basic slime enemy AI:
/// - Patrols randomly within a range of its spawn point
/// - Detects and chases the player if within range
/// - Attacks the player while colliding
/// - Takes damage and dies when health reaches zero
/// </summary>
public class Slime : MonoBehaviour, IDamageable
{

    /// <summary>
    /// Possible states for the slime AI.
    /// </summary>
    private enum SlimeState { Patrolling, Chasing }
    private SlimeState currentState;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 15;
    [SerializeField] private int attackDamage = 5; 
    private int currentHealth;

    [Header("AI Ranges")]
    [SerializeField] private float detectionRange = 8f; 
    [SerializeField] private float patrolRange = 5f;     

    [Header("Movement AI")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float hopInterval = 2f;
    [SerializeField] private float hopDuration = 0.5f;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Vector2 startPosition; // The slime's spawn point

    [Header("Effects")]
    [SerializeField] private Color damageFlashColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Color originalColor;
    private bool isAttacking = false;
    private PlayerStats playerStats;

    /// <summary>
    /// Called when the object is first created.
    /// Initializes core component references (Rigidbody, SpriteRenderer, Animator).
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Called on the first frame.
    /// - Sets health and state
    /// - Finds the player
    /// - Starts the slime's hopping AI coroutine
    /// </summary>
    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position; 
        currentState = SlimeState.Patrolling; 

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        StartCoroutine(HopRoutine());
    }

    /// <summary>
    /// Runs every frame:
    /// - Decides whether to patrol or chase based on distance to player
    /// - Applies damage if currently colliding with the player
    /// </summary>
    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            currentState = SlimeState.Chasing;
        }
        else
        {
            currentState = SlimeState.Patrolling;
        }
        if(isAttacking) playerStats.TakeDamage(attackDamage);
    }

    // --- AI & Movement ---
    
    /// <summary>
    /// Coroutine that controls the slime's "hop" movement.
    /// - Waits for hopInterval seconds
    /// - Picks direction (toward player if chasing, random if patrolling)
    /// - Moves for hopDuration seconds
    /// - Stops, then repeats
    /// </summary>
    private IEnumerator HopRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(hopInterval);

            Vector2 direction;

            if (currentState == SlimeState.Chasing && playerTransform != null)
            {
                direction = (playerTransform.position - transform.position).normalized;
            }
            else
            {
                Vector2 randomDirection = Random.insideUnitCircle * patrolRange;
                Vector2 targetPosition = startPosition + randomDirection;
                direction = (targetPosition - (Vector2)transform.position).normalized;
            }

            if (direction.x < 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x > 0)
            {
                spriteRenderer.flipX = true; 
            }

            anim.SetBool("isHopping", true);

            rb.linearVelocity = direction * moveSpeed;
            yield return new WaitForSeconds(hopDuration);

            anim.SetBool("isHopping", false);
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    // --- Collision Handling ---

    /// <summary>
    /// Called when slime enters a trigger collider.
    /// If collider is the player, set isAttacking = true so damage is applied in Update().
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = true;
        }
    }
    
    /// <summary>
    /// Called when slime exits a trigger collider.
    /// If collider is the player, stop applying damage.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {     
            isAttacking = false;
        }
    }

    // --- Damage & Death ---

    /// <summary>
    /// Applies damage to slime, flashes sprite, and calls Die() if HP <= 0.
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0) Die();
    }

    /// <summary>
    /// Briefly changes slime’s sprite color to indicate damage.
    /// </summary>
    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// Handles slime’s death:
    /// - Updates achievement/stat systems
    /// - Plays death animation
    /// - Disables collider and behavior
    /// - Destroys slime object after delay
    /// </summary>
    private void Die()
    {
        AchievementManager.Instance.AddProgress("001", 1);
        StatisticsManager.Increase("enemiesKilled");
        anim.SetTrigger("Die");
        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 1f);
    }

    /// <summary>
    /// Draws patrol range (blue) and detection range (yellow) in the editor.
    /// Helps visualize AI behavior zones.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw Patrol Range (Blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, patrolRange);
        // Draw Detection Range (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}