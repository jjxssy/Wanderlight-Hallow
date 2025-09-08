using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls a boss enemy with multiple states (Idle, Chasing, Attacking, Dead).
/// Includes AI logic for two phases:
/// - Phase 1: Patrols, chases the player, and performs ranged attacks
/// - Phase 2: Unlocks a spinning radial projectile attack
/// Handles health, damage, death, and a boss health bar UI.
/// </summary>

public class BossEnemy : MonoBehaviour, IDamageable
{
    /// <summary>
    /// Internal state machine for the boss.
    /// </summary>
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

    /// <summary>
    /// Unity Awake: caches components and setup.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Unity Start: initializes stats, finds player, sets Idle state, starts AI coroutine.
    /// </summary>
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

    /// <summary>
    /// Unity Update: handles animations and health bar updates.
    /// </summary>
    void Update()
    {
        HandleMovementAnimation();
        UpdateHealthBar();
    }

    /// <summary>
    /// Sets animation parameters based on movement velocity.
    /// </summary>
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

    /// <summary>
    /// Main AI coroutine handling patrol, chase, and attacks.
    /// Switches to phase 2 if health drops below 50%.
    /// </summary>
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
                            yield return StartCoroutine(RangedAttack());
                        }
                        rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
                        break;

                    case BossState.Attacking:
                        rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
                        break;
                }
            }
            else
            {
                rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
            }
            yield return null;
        }
    }


    /// <summary>
    /// Performs a single ranged attack using the boss's projectile skill.
    /// </summary>
    private IEnumerator RangedAttack()
    {
        currentState = BossState.Attacking;
        lastAttackTime = Time.time;
        yield return new WaitForSeconds(0.5f);
        if (bossProjectileSkill != null && projectileSpawnPoint != null)
        {
            GameObject projGO = Instantiate(bossProjectileSkill.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            if (bossProjectileSkill.vfxPrefab != null)
            {
                Instantiate(bossProjectileSkill.vfxPrefab, projGO.transform.position, projGO.transform.rotation, projGO.transform);
            }

            Projectile projectile = projGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                Vector2 direction = (playerTransform.position - projectileSpawnPoint.position).normalized;
                projectile.Initialize(direction, bossProjectileSkill.projectileSpeed, bossProjectileSkill.damage, 0, bossProjectileSkill.orientPerpendicular);
            }
        }

        yield return new WaitForSeconds(0.5f);
        currentState = BossState.Idle;
    }
    /// <summary>
    /// Coroutine for phase 2 spinning radial attack.
    /// Fires a circle of projectiles around the boss every cooldown cycle.
    /// </summary>

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
                if (currentState == BossState.Dead) break; 


                float currentAngle = i * angleStep;
                Vector2 direction = new Vector2(Mathf.Sin(currentAngle * Mathf.Deg2Rad), Mathf.Cos(currentAngle * Mathf.Deg2Rad));

                if (bossProjectileSkill != null && projectileSpawnPoint != null)
                {
                    GameObject projGO = Instantiate(bossProjectileSkill.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                    if (bossProjectileSkill.vfxPrefab != null)
                    {
                        Instantiate(bossProjectileSkill.vfxPrefab, projGO.transform.position, projGO.transform.rotation, projGO.transform);
                    }
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

    /// <summary>
    /// Applies damage to the boss and handles phase transition and death.
    /// </summary>
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

    /// <summary>
    /// Handles boss death: stop AI, disable collider, and destroy object.
    /// </summary>
    private void Die()
    {
        currentState = BossState.Dead;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        StopAllCoroutines();
        Destroy(gameObject, 2f);
    }

    /// <summary>
    /// Updates the UI health bar to reflect current health.
    /// </summary>
    private void UpdateHealthBar()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }
    }

    /// <summary>
    /// Coroutine that briefly flashes the boss red when taking damage.
    /// </summary>
    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// Draws debug gizmos for detection and patrol ranges in the editor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (Application.isPlaying) Gizmos.color = Color.blue;
        else Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPosition, patrolRange);
    }
}