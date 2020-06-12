using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Character attributes:")]
    public float MOVEMENT_BASE_SPEED = 1.0f;

    [Space]
    [Header("Character statistics:")]
    public Vector2 movementDirection;
    public float movementSpeed;

    private Rigidbody2D rb;
    private Animator animator;

    // From variables to fix animation inconsistencies
    private bool fromX, fromY;

    private Vector2 lastMovementDirection = Vector2.zero;

    private bool waitFrameOver = false;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (!PauseSystem.paused) {
            
            // Wait frame added to prevent keystate loss between scenes
            if (waitFrameOver)
            {
                ProcessInputs();
                Move();
                Animate();
            }
            
            waitFrameOver = true;
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
    }

    void ProcessInputs()
    {
        // Set the movement direction to the player input
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Set the movement speed to the magnitude of the movement direction
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);

        // Normalize the movement direction
        movementDirection.Normalize();
    }

    void Move()
    {
        // Move the player by changing the velocity of the rigidbody
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;
    }

    void Animate()
    {
        // Reset from paramaters when player not moving
        if (movementDirection.x == 0) fromX = false;
        if (movementDirection.y == 0) fromY = false;

        // Set animator values if player moving
        if (movementDirection != Vector2.zero) {
        
            lastMovementDirection = movementDirection;

            // If moving horizontally and not previously moving vertically
            if (movementDirection.x != 0 && !fromY)
            {
                fromX = true;
                animator.SetFloat("Horizontal", movementDirection.x);
            }
            else
            {
                animator.SetFloat("Horizontal", 0);
            }

            // If moving vertically and not previously moving horizontally
            if (movementDirection.y != 0 && !fromX)
            {
                fromY = true;
                animator.SetFloat("Vertical", movementDirection.y);
            }
            else
            {
                animator.SetFloat("Vertical", 0);
            }
        
        }

        // Set speed of animator to movement speed
        animator.SetFloat("Speed", movementSpeed);
    }
}
