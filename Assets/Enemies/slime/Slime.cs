using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour, IDamageable
{

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
    }

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isAttacking = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {     
            isAttacking = false;
        }
    }

    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0) Die();
    }

    private IEnumerator DamageFlash()
    {

        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        anim.SetTrigger("Die");

        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 1f);
    }


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