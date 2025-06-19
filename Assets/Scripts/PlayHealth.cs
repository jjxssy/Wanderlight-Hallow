using UnityEngine;
using UnityEngine.UI;
public class PlayHealth : MonoBehaviour
{
    [SerializeField]private int maxHealth=10;
    [SerializeField]private int health;
    [SerializeField]private Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health=maxHealth;
        slider.maxValue=maxHealth;
        slider.value=health;
    }

    public void takeDamage(int damage)
    {
        health-=damage;
        slider.value=health;
        if(health<=0)
            Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
