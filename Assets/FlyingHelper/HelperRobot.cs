using UnityEngine;

/// <summary>
/// Autonomous ally that follows the player at a comfortable distance and,
/// when a boss is within range, rotates toward it and fires projectiles at a
/// configurable fire rate using a <see cref="ProjectileSkill"/>.
/// </summary>
public class HelperRobot : MonoBehaviour
{
    [Header("Movement Settings")]
    /// <summary>Movement speed while following the player.</summary>
    [SerializeField] private float followSpeed = 3f;

    /// <summary>Maximum distance at which the robot will attempt to shoot the boss.</summary>
    [SerializeField] private float shootingRange = 10f;

    /// <summary>Desired distance to keep from the player before stopping.</summary>
    [SerializeField] private float stoppingDistance = 3f;

    /// <summary>Smoothing factor for turning to face the boss.</summary>
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Combat Settings")]
    /// <summary>Skill that defines projectile prefab, speed, damage and orientation behavior.</summary>
    [SerializeField] private ProjectileSkill projectileSkill;

    /// <summary>Transform from which projectiles are spawned.</summary>
    [SerializeField] private Transform projectileSpawnPoint;

    /// <summary>Shots per second when the boss is in range.</summary>
    [SerializeField] private float fireRate = 1f;

    /// <summary>Cached reference to the player's transform (looked up by tag "Player").</summary>
    private Transform playerTransform;

    /// <summary>Cached reference to the boss's transform (looked up by tag "Boss").</summary>
    private Transform bossTransform;

    /// <summary>Next world-time timestamp when another shot is allowed.</summary>
    private float nextFireTime;

    /// <summary>
    /// Finds the player and (optionally present) boss by tag on startup and caches their transforms.
    /// </summary>
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        bossTransform = GameObject.FindGameObjectWithTag("Boss")?.transform;
    }

    /// <summary>
    /// Each frame: follow the player and, if a boss exists and is in range, rotate and fire.
    /// </summary>
    void Update()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        FollowPlayer();
        LookForBoss();
    }

    /// <summary>
    /// Moves toward the player until within <see cref="stoppingDistance"/>; otherwise stays put.
    /// </summary>
    void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) > stoppingDistance)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, followSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Acquires the boss if missing, checks distance, smoothly rotates to face it,
    /// and fires at the configured <see cref="fireRate"/> while in range.
    /// </summary>
    void LookForBoss()
    {
        if (bossTransform == null)
        {
            bossTransform = GameObject.FindGameObjectWithTag("Boss")?.transform;
            return;
        }

        float distanceToBoss = Vector3.Distance(transform.position, bossTransform.position);

        if (distanceToBoss <= shootingRange)
        {
            Vector3 directionToBoss = (bossTransform.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToBoss.y, directionToBoss.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    /// <summary>
    /// Spawns a projectile via the configured <see cref="ProjectileSkill"/> and initializes its trajectory toward the boss.
    /// </summary>
    void Shoot()
    {
        if (projectileSkill != null && projectileSpawnPoint != null)
        {
            GameObject projGO = Instantiate(projectileSkill.GetProjectilePrefab(), projectileSpawnPoint.position, Quaternion.identity);
            var vfx = projectileSkill.GetVfxPrefab();
            if (vfx != null)
            {
                Instantiate(vfx, projGO.transform.position, projGO.transform.rotation, projGO.transform);
            }

            Projectile projectile = projGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                Vector2 direction = (bossTransform.position - projectileSpawnPoint.position).normalized;
                projectile.Initialize(direction,
                                      projectileSkill.GetProjectileSpeed(),
                                      projectileSkill.GetDamage(),
                                      0f,
                                      projectileSkill.GetOrientPerpendicular());
            }
        }
    }
}
