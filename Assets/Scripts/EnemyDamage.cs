using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField]private int damage;
    [SerializeField]private PlayHealth PlayHealth;


    private void OnCollisionEnter2D(Collision2D Collision)
    {
        if(Collision.gameObject.tag=="Player")
        {
            PlayHealth.takeDamage(damage);
        }
    }
}
