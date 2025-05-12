using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    public Animator animator;

    void Start()
    {
        animator=GetComponent<Animator>();
    }
    // Update is called once per frame
    // frame rate can change any min so for physics it is not relayable 
    // so we gonna use a function called fixed update
   // it function like update but it function on a fixed timmer 
    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
        animator.SetFloat("lastInputX" ,  movement.x);
        animator.SetFloat("lastInputY" ,  movement.y);
        animator.SetFloat("inputX" ,movement.x);
        animator.SetFloat("inputY" ,movement.y);
        animator.SetFloat("speed" , movement.sqrMagnitude);
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
    
}
