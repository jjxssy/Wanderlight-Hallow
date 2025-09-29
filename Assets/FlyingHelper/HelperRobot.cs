using UnityEngine;

public class HelperRobot : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float shootingRange = 10f;
    [SerializeField] private float stoppingDistance = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Combat Settings")]
    [SerializeField] private ProjectileSkill projectileSkill;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float fireRate = 1f;

    private Transform playerTransform;
    private Transform bossTransform;
    private float nextFireTime;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        bossTransform = GameObject.FindGameObjectWithTag("Boss")?.transform;
    }

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

    void FollowPlayer()
    {

        if (Vector3.Distance(transform.position, playerTransform.position) > stoppingDistance)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, followSpeed * Time.deltaTime);
        }
    }

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
            if(projectile != null)
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