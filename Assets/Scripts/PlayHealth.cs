using UnityEngine;

public class PlayHealth : MonoBehaviour
{
    [SerializeField]private int maxHealth=10;
    [SerializeField]private int health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health=maxHealth;
    }

    public void takeDamage(int damage)
    {
        health-=damage;
        if(health<=0)
            Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
