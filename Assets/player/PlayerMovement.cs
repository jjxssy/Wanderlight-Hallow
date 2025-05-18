using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    private float speed = 3f;
    private Rigidbody2D rb;
    Vector2 movement;
    private Animator animator;

    void Start()
    {   
        rb = GetComponent<Rigidbody2D>();
        animator=GetComponent<Animator>();
    }
    // Update is called once per frame
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
