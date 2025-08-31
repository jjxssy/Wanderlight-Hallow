using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour, IDamageable
{
    private enum BossState { Idle, Chasing, Attacking, Dead }
    private BossState currentState;

    [Header("Core Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float patrolRange = 7f;

    [Header("Attack Settings")]
    [SerializeField] private ProjectileSkill bossProjectileSkill;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float attackCooldown = 3f;

    [Header("Phase 2 Attack Settings")]
    [SerializeField] private float phaseTwoAttackCooldown = 4f;
    [SerializeField] private float phaseTwoSpinFireRate = 0.05f;

    [Header("Visuals & UI")]
    [SerializeField] private Slider bossHealthBar;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Color originalColor;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isInPhaseTwo = false;

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
        UpdateHealthBar();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        currentState = BossState.Idle;
        StartCoroutine(BossAI());
    }

    void Update()
    {
        HandleMovementAnimation();
        UpdateHealthBar();
    }

    private void HandleMovementAnimation()
    {
        if (currentState == BossState.Dead) return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            anim.speed = 1;
            anim.SetFloat("moveX", rb.linearVelocity.normalized.x);
            anim.SetFloat("moveY", rb.linearVelocity.normalized.y);
        }
        else
        {
            anim.speed = 0;
        }
    }

    private IEnumerator BossAI()
    {
        yield return new WaitUntil(() => playerTransform != null);

        while (currentState != BossState.Dead)
        {
            if (!isInPhaseTwo)
            {
                // --- PHASE 1 MOVEMENT AND STATE LOGIC ---
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                if (currentState != BossState.Attacking)
                {
                    currentState = (distanceToPlayer <= detectionRange) ? BossState.Chasing : BossState.Idle;
                }

                switch (currentState)
                {
                    case BossState.Idle:
                        Vector2 randomTarget = startPosition + (Random.insideUnitCircle * patrolRange);
                        rb.linearVelocity = (randomTarget - (Vector2)transform.position).normalized * moveSpeed;
                        yield return new WaitUntil(() => Vector2.Distance(transform.position, randomTarget) < 1f || currentState != BossState.Idle);
                        rb.linearVelocity = Vector2.zero;
                        yield return new WaitForSeconds(2f);
                        break;

                    case BossState.Chasing:
                        if (Time.time >= lastAttackTime + attackCooldown)
                        {
                            rb.linearVelocity = Vector2.zero;
                            yield return StartCoroutine(RangedAttack());
                        }
                        else
                        {
                            rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
                        }
                        break;
                }
            }
            else
            {
                // --- PHASE 2 MOVEMENT LOGIC ---
                // In Phase 2, the AI is simple: always chase the player.
                // The attack is handled by a separate, parallel coroutine.
                rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
            }
            yield return null;
        }
    }

    private IEnumerator RangedAttack()
    {
        currentState = BossState.Attacking;
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(0.5f);

        if (bossProjectileSkill != null && projectileSpawnPoint != null)
        {
            Vector2 direction = (playerTransform.position - projectileSpawnPoint.position).normalized;
            GameObject projGO = Instantiate(bossProjectileSkill.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Projectile projectile = projGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(direction, bossProjectileSkill.projectileSpeed, bossProjectileSkill.damage, 0, bossProjectileSkill.orientPerpendicular);
            }
        }

        yield return new WaitForSeconds(0.5f);
        currentState = BossState.Idle;
    }

    // This new coroutine runs independently to handle the Phase 2 attack
    private IEnumerator PhaseTwoAttackRoutine()
    {
        while (currentState != BossState.Dead)
        {
            // Wait for the attack cooldown
            yield return new WaitForSeconds(phaseTwoAttackCooldown);

            // Perform the spinning radial attack
            int numberOfProjectiles = 36;
            float angleStep = 360f / numberOfProjectiles;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                if (currentState == BossState.Dead) break; // Stop shooting if the boss dies mid-attack

                float currentAngle = i * angleStep;
                Vector2 direction = new Vector2(Mathf.Sin(currentAngle * Mathf.Deg2Rad), Mathf.Cos(currentAngle * Mathf.Deg2Rad));

                if (bossProjectileSkill != null && projectileSpawnPoint != null)
                {
                    GameObject projGO = Instantiate(bossProjectileSkill.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                    Projectile projectile = projGO.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        projectile.Initialize(direction, bossProjectileSkill.projectileSpeed, bossProjectileSkill.damage, 0, bossProjectileSkill.orientPerpendicular);
                    }
                }
                yield return new WaitForSeconds(phaseTwoSpinFireRate);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        StartCoroutine(DamageFlash());

        // Check if we should enter Phase 2
        if (!isInPhaseTwo && currentHealth <= maxHealth / 2)
        {
            isInPhaseTwo = true;
            Debug.Log("Boss has entered Phase 2!");

            // Start the independent attack coroutine. It will now run forever alongside the main AI.
            StartCoroutine(PhaseTwoAttackRoutine());
        }

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        currentState = BossState.Dead;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        StopAllCoroutines();
        Destroy(gameObject, 2f);
    }

    private void UpdateHealthBar()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (Application.isPlaying) Gizmos.color = Color.blue;
        else Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPosition, patrolRange);
    }
}