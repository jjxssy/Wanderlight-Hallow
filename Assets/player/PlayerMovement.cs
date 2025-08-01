using UnityEngine;

/// <summary>
/// Handles basic player movement using input axes and applies motion via Rigidbody2D.
/// Also updates animator parameters for 4-directional animation.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get player input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Update animator parameters
        animator.SetFloat("inputX", movement.x);
        animator.SetFloat("inputY", movement.y);
        animator.SetFloat("speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("lastInputX", movement.x);
            animator.SetFloat("lastInputY", movement.y);
        }
    }

    private void FixedUpdate()
    {
        // Apply movement to Rigidbody2D
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Allows other scripts to modify movement speed (e.g., by items).
    /// </summary>
    /// <param name="value">Amount to add/subtract from base speed</param>
    public void AddSpeed(float value)
    {
        moveSpeed += value;
    }

    /// <summary>
    /// Gets the current movement speed.
    /// </summary>
    public float GetSpeed()
    {
        return moveSpeed;
    }
}
