using UnityEngine;
public class WorldSkill : MonoBehaviour
{
    [SerializeField] private Skill skill;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerSkillManager>().LearnSkill(skill);
            Destroy(gameObject);
        }
    }
}
