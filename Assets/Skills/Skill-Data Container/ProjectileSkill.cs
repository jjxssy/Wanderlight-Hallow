using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Skill", menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Combat Stats")]
    public int damage = 10;
    public float pushbackForce = 0f;
    [Header("Orientation")]
    public bool orientPerpendicular = false;

    public override void Activate(GameObject user)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - user.transform.position).normalized;

        GameObject projectileInstance = Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);
        Projectile projectile = projectileInstance.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(direction, projectileSpeed, damage, pushbackForce,orientPerpendicular);
        }
    }



    public override string GetStatDetails()
    {
        return $"Damage: {damage}";
    }
}