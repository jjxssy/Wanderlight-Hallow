using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Top–down boss controller with two phases:
/// Phase 1 patrols/chases and fires aimed projectiles; at ≤50% HP,
/// Phase 2 adds a periodic radial spin attack running alongside the base AI.
/// Handles health, movement, attacks, hit flash, death, and a UI health bar.
/// </summary>
public class BossEnemy : MonoBehaviour, IDamageable
{
    /// <summary>Finite states for the boss behavior loop.</summary>
    private enum BossState { Idle, Chasing, Attacking, Dead }

    /// <summary>Current state of the boss AI.</summary>
    private BossState currentState;

    #region Core Stats
    [Header("Core Stats")]
    /// <summary>Maximum health of the boss.</summary>
    [SerializeField] private int maxHealth = 100;
    /// <summary>Runtime health value (clamped 0..maxHealth).</summary>
    private int currentHealth;
    /// <summary>Chase/patrol speed (units/sec).</summary>
    [SerializeField] private float moveSpeed = 2f;
    /// <summary>Distance at which the boss begins chasing the player.</summary>
    [SerializeField] private float detectionRange = 10f;
    /// <summary>Radius around the start position used for idle/patrol wandering.</summary>
    [SerializeField] private float patrolRange = 7f;
    #endregion

    #region Phase 1 Attack
    [Header("Attack Settings")]
    /// <summary>Projectile skill used for aimed shots at the player.</summary>
    [SerializeField] private ProjectileSkill bossProjectileSkill;
    /// <summary>Transform used as the projectile spawn origin.</summary>
    [SerializeField] private Transform projectileSpawnPoint;
    /// <summary>Cooldown between aimed attacks (seconds).</summary>
    [SerializeField] private float attackCooldown = 3f;
    #endregion

    #region Phase 2 Attack
    [Header("Phase 2 Attack Settings")]
    /// <summary>Delay between radial spin volleys during phase 2 (seconds).</summary>
    [SerializeField] private float phaseTwoAttackCooldown = 4f;
    /// <summary>Delay between projectiles within a single radial volley (seconds).</summary>
    [SerializeField] private float phaseTwoSpinFireRate = 0.05f;
    #endregion

    #region Visuals & UI
    [Header("Visuals & UI")]
    /// <summary>World-space or screen-space health bar for the boss.</summary>
    [SerializeField] private Slider bossHealthBar;
    /// <summary>Cached renderer for damage flash.</summary>
    private SpriteRenderer spriteRenderer;
    /// <summary>Cached animator for movement facing/idle control.</summary>
    private Animator anim;
    /// <summary>Original sprite color restored after hit flash.</summary>
    private Color originalColor;
    #endregion

    /// <summary>Reference to player transform (resolved at runtime).</summary>
    private Transform playerTransform;
    /// <summary>Cached rigidbody used for movement.</summary>
    private Rigidbody2D rb;
    /// <summary>Start point used for idle patrol wandering.</summary>
    private Vector2 startPosition;
    /// <summary>Timestamp of the last aimed attack.</summary>
    private float lastAttackTime = -Mathf.Infinity;
    /// <summary>Flag indicating phase 2 has begun (≤ 50% HP).</summary>
    private bool isInPhaseTwo = false;
    //For questing
    [SerializeField] QuestNPC relatedNPC;

    /// <summary>Cache components and baseline visuals.</summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
    }

    /// <summary>Initialize health/UI, find player, set initial state, and start the AI loop.</summary>
    private void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        UpdateHealthBar();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        currentState = BossState.Idle;
        StartCoroutine(BossAI());
    }

    /// <summary>Per-frame visuals (movement animation) and UI syncing.</summary>
    private void Update()
    {
        HandleMovementAnimation();
        UpdateHealthBar();
    }

    /// <summary>Updates animator parameters based on current velocity; stops anim when idle.</summary>
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

    /// <summary>Main AI coroutine handling state transitions, patrol, chase, and attack cadence.</summary>
    private IEnumerator BossAI()
    {
        // Wait until player exists
        yield return new WaitUntil(() => playerTransform != null);
        while (currentState != BossState.Dead)
        {
            if (!isInPhaseTwo)
            {
                // Decide state based on distance unless in an attack wind-up
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                if (currentState != BossState.Attacking)
                {
                    currentState = (distanceToPlayer <= detectionRange) ? BossState.Chasing : BossState.Idle;
                }

                switch (currentState)
                {
                    case BossState.Idle:
                        // Wander to a random point around the start position
                        Vector2 randomTarget = startPosition + (Random.insideUnitCircle * patrolRange);
                        rb.linearVelocity = (randomTarget - (Vector2)transform.position).normalized * moveSpeed;
                        yield return new WaitUntil(() =>
                            Vector2.Distance(transform.position, randomTarget) < 1f || currentState != BossState.Idle);
                        rb.linearVelocity = Vector2.zero;
                        yield return new WaitForSeconds(2f);
                        break;

                    case BossState.Chasing:
                        // Fire if off cooldown, otherwise chase
                        if (Time.time >= lastAttackTime + attackCooldown)
                        {
                            yield return StartCoroutine(RangedAttack());
                        }
                        rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
                        break;

                    case BossState.Attacking:
                        // Keep drifting toward player during attack wind-up
                        rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
                        break;
                }
            }
            else
            {
                // Phase 2: continuous chase while the spin attack runs on its own coroutine
                rb.linearVelocity = (playerTransform.position - transform.position).normalized * moveSpeed;
            }
            yield return null;
        }
    }

    /// <summary>Wind-up, spawn projectile, and fire toward the player using the configured skill.</summary>
    private IEnumerator RangedAttack()
    {
        currentState = BossState.Attacking;
        lastAttackTime = Time.time;
        yield return new WaitForSeconds(0.5f);
        if (bossProjectileSkill != null && projectileSpawnPoint != null)
        {
            GameObject projGO = Instantiate(bossProjectileSkill.GetProjectilePrefab(), projectileSpawnPoint.position, Quaternion.identity);

            var vfx = bossProjectileSkill.GetVfxPrefab();
            if (vfx != null)
            {
                Instantiate(vfx, projGO.transform.position, projGO.transform.rotation, projGO.transform);
            }

            Projectile projectile = projGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                Vector2 direction = (playerTransform.position - projectileSpawnPoint.position).normalized;
                projectile.Initialize(direction,
                                      bossProjectileSkill.GetProjectileSpeed(),
                                      bossProjectileSkill.GetDamage(),
                                      0f,
                                      bossProjectileSkill.GetOrientPerpendicular());
            }
        }

        yield return new WaitForSeconds(0.5f);
        currentState = BossState.Idle;
    }

    /// <summary>Phase 2 routine: periodically emits a ring of projectiles in all directions.</summary>
    private IEnumerator PhaseTwoAttackRoutine()
    {
        while (currentState != BossState.Dead)
        {
            yield return new WaitForSeconds(phaseTwoAttackCooldown);

            int numberOfProjectiles = 36;
            float angleStep = 360f / numberOfProjectiles;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                if (currentState == BossState.Dead) break;

                float currentAngle = i * angleStep;
                Vector2 direction = new Vector2(Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                                                Mathf.Cos(currentAngle * Mathf.Deg2Rad));

                if (bossProjectileSkill != null && projectileSpawnPoint != null)
                {
                    GameObject projGO = Instantiate(bossProjectileSkill.GetProjectilePrefab(), projectileSpawnPoint.position, Quaternion.identity);

                    var vfx = bossProjectileSkill.GetVfxPrefab();
                    if (vfx != null)
                    {
                        Instantiate(vfx, projGO.transform.position, projGO.transform.rotation, projGO.transform);
                    }

                    Projectile projectile = projGO.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        projectile.Initialize(direction,
                                              bossProjectileSkill.GetProjectileSpeed(),
                                              bossProjectileSkill.GetDamage(),
                                              0f,
                                              bossProjectileSkill.GetOrientPerpendicular());
                    }
                }
                yield return new WaitForSeconds(phaseTwoSpinFireRate);
            }
        }
    }

    /// <summary>Applies incoming damage, triggers hit flash, enters phase 2 at ≤50% HP, and handles death.</summary>
    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Dead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        StartCoroutine(DamageFlash());

        // Enter phase 2 once
        if (!isInPhaseTwo && currentHealth <= maxHealth / 2)
        {
            isInPhaseTwo = true;
            Debug.Log("Boss has entered Phase 2!");
            StartCoroutine(PhaseTwoAttackRoutine());
        }

        if (currentHealth <= 0) Die();
    }

    /// <summary>Stops AI/movement, disables collisions, and destroys the boss after a short delay.</summary>
    private void Die()
    {
        if (relatedNPC != null)
        {
            relatedNPC.OnBossDefeated();
        }
        currentState = BossState.Dead;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        StopAllCoroutines();
        Destroy(gameObject, 2f);
    }

    /// <summary>Writes current/max health to the assigned slider if present.</summary>
    private void UpdateHealthBar()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = currentHealth;
        }
    }

    /// <summary>Brief red flash on damage for visual feedback.</summary>
    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    /// <summary>Editor helper: shows detection and patrol radii.</summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Application.isPlaying ? Color.blue : Color.cyan;
        Gizmos.DrawWireSphere(startPosition, patrolRange);
    }
}
