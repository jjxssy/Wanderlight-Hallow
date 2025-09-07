using UnityEngine;

/// <summary>
/// Handles player movement and animation updates in a 2D top-down environment.
/// Uses Rigidbody2D for movement and Animator for direction/speed updates.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Reference to the Rigidbody2D component used for movement.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Reference to the Animator controlling player animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Current movement input vector (X and Y axes).
    /// </summary>
    private Vector2 movement;

    /// <summary>
    /// Initializes component references.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Handles input collection and updates animator parameters.
    /// </summary>
    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("inputX", movement.x);
        animator.SetFloat("inputY", movement.y);
        animator.SetFloat("speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("lastInputX", movement.x);
            animator.SetFloat("lastInputY", movement.y);
        }
    }

    /// <summary>
    /// Moves the player using Rigidbody2D physics based on input and PlayerStats speed.
    /// </summary>
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * PlayerStats.instance.GetSpeed() * Time.fixedDeltaTime);
    }
}
