using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Skill", menuName = "Skills/Projectile Skill")]
public class ProjectileSkill : Skill
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    public override void Activate(GameObject user)
    {
        //For now, let's assume it shoots towards the mouse cursor.
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector2 direction = (mousePosition - user.transform.position).normalized;

        GameObject projectileInstance = Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);
        Projectile projectile = projectileInstance.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(direction, projectileSpeed);
        }
    }
}