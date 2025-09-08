using UnityEngine;

/// <summary>
/// A concrete skill that spawns and launches a projectile toward the mouse cursor.
/// Uses private serialized fields with Java-style getters/setters,
/// and optional VFX that attach to the spawned projectile.
/// </summary>
[CreateAssetMenu(fileName = "New Projectile Skill", menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{
    [Header("Prefabs")]
    [Tooltip("The object with the Rigidbody2D and Projectile.cs script.")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float pushbackForce = 0f;
    [SerializeField] private bool orientPerpendicular = false;

    /// <summary>Gets the projectile prefab this skill spawns.</summary>
    public GameObject GetProjectilePrefab() { return projectilePrefab; }
    /// <summary>Sets the projectile prefab this skill spawns.</summary>
    public void SetProjectilePrefab(GameObject value) { projectilePrefab = value; }

    /// <summary>Gets the launch speed of the projectile (units/second).</summary>
    public float GetProjectileSpeed() { return projectileSpeed; }
    /// <summary>Sets the launch speed of the projectile (units/second).</summary>
    public void SetProjectileSpeed(float value) { projectileSpeed = value; }

    /// <summary>Gets the damage applied by the projectile on hit.</summary>
    public int GetDamage() { return damage; }
    /// <summary>Sets the damage applied by the projectile on hit.</summary>
    public void SetDamage(int value) { damage = value; }

    /// <summary>Gets the impulse force applied to targets on impact.</summary>
    public float GetPushbackForce() { return pushbackForce; }
    /// <summary>Sets the impulse force applied to targets on impact.</summary>
    public void SetPushbackForce(float value) { pushbackForce = value; }

    /// <summary>
    /// Gets whether the projectile visual should be rotated +90° relative to its travel direction.
    /// Useful if the sprite faces up by default.
    /// </summary>
    public bool GetOrientPerpendicular() { return orientPerpendicular; }
    /// <summary>
    /// Sets whether the projectile visual should be rotated +90° relative to its travel direction.
    /// </summary>
    public void SetOrientPerpendicular(bool value) { orientPerpendicular = value; }

    /// <summary>
    /// Spawns a projectile at the user position and launches it toward the mouse.
    /// Attaches optional VFX as a child of the projectile, and initializes damage/pushback/orientation.
    /// </summary>
    /// <param name="user">The GameObject (usually the player) activating this skill.</param>
    public override void Activate(GameObject user)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 direction = (mousePosition - user.transform.position).normalized;

        GameObject prefab = GetProjectilePrefab();
        if (prefab == null) return;

        GameObject projectileInstance = Object.Instantiate(prefab, user.transform.position, Quaternion.identity);

        GameObject vfx = GetVfxPrefab();
        if (vfx != null)
        {
            Object.Instantiate(
                vfx,
                projectileInstance.transform.position,
                projectileInstance.transform.rotation,
                projectileInstance.transform
            );
        }

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                GetProjectileSpeed(),
                GetDamage(),
                GetPushbackForce(),
                GetOrientPerpendicular()
            );
        }
    }

    /// <summary>
    /// Returns a short UI string describing the projectile's damage output.
    /// </summary>
    public override string GetStatDetails()
    {
        return $"Damage: {GetDamage()}";
    }
}
