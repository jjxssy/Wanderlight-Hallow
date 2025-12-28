using UnityEngine;

/// <summary>
/// World pickup that grants a <see cref="Skill"/> to the player on contact,
/// then removes itself from the scene.
/// Place this on a trigger collider with a Skill assigned in the Inspector.
/// </summary>
public class WorldSkill : MonoBehaviour
{
    [SerializeField] private Skill skill;

    /// <summary>
    /// When the player enters the trigger, teaches them the configured skill
    /// via <see cref="PlayerSkillManager.LearnSkill(Skill)"/> and destroys this pickup.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerSkillManager>().LearnSkill(skill);
            Destroy(gameObject);
        }
    }
}
