using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Skill", menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{
    [Header("Prefabs")]
    [Tooltip("The object with the Rigidbody2D and Projectile.cs script.")]
    public GameObject projectilePrefab;
    [Tooltip("The object with the particle systems for the visuals.")]

    [Header("Settings")]
    public float projectileSpeed = 10f;
    public int damage = 10;
    public float pushbackForce = 0f;
    public bool orientPerpendicular = false;

    public override void Activate(GameObject user)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - user.transform.position).normalized;

        GameObject projectileInstance = Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);


        if (vfxPrefab != null)
        {
            GameObject vfxInstance = Instantiate(vfxPrefab, projectileInstance.transform.position, projectileInstance.transform.rotation, projectileInstance.transform);
        }

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, projectileSpeed, damage, pushbackForce, orientPerpendicular);
        }
    }

    public override string GetStatDetails()
    {
        return $"Damage: {damage}";
    }
}