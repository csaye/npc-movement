using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("References: ")]
    public NPCScriptable NPCScriptable;
    
    private Rigidbody2D rb;
    private Animator animator;

    private List<Vector3> path;
    private int currentPathIndex;

    private Vector2 currentTarget;
    private float nextTargetHour;

    private float lastMoveTime;

    private float errorMargin = 0.01f;

    private bool rollover = false;

    private float lastGameTime;

    void Start()
    {
        if (path == null)
        {
            path = NPCScriptable.path;
        }
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        float currentHour = (float)TimeSystem.gameTime.TotalHours;

        // Get the next target position and time
        for (int i = 0; i < path.Count; i++)
        {
            float pathHour = path[i].z;

            if (pathHour <= currentHour)
            {
                continue;
            }
            else
            {
                nextTargetHour = pathHour;
                currentTarget = new Vector2(path[i].x, path[i].y);
                currentPathIndex = i;

                break;
            }
        }
    }

    void Update()
    {
        if (!PauseSystem.paused)
        {
            CheckRollover();
            
            // If next target hour not reached
            if ((!rollover && (TimeSystem.gameTime.TotalHours < nextTargetHour)) || (rollover && (TimeSystem.gameTime.TotalHours - 24 < nextTargetHour)))
            {
                // If at target, stop
                if (Mathf.Abs(transform.position.x - currentTarget.x) < errorMargin && Mathf.Abs(transform.position.y - currentTarget.y) < errorMargin)
                {
                    rb.velocity = Vector2.zero;

                    animator.SetFloat("Speed", 0);

                    // Snap to grid position for accuracy
                    transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
                }
                else
                // Move to target
                {
                    if (Time.time - lastMoveTime >= 1)
                    {
                        MoveToTarget(currentTarget);

                        // Snap to grid position for accuracy
                        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
                    }
                }
            }
            else
            {
                // Find the next target position and time
                if (currentPathIndex == path.Count - 1)
                {
                    currentPathIndex = 0;
                }
                else
                {
                    currentPathIndex++;
                }
                
                // Set to rollover if new target hour is less than current target hour
                if (path[currentPathIndex].z < nextTargetHour)
                {
                    rollover = true;
                }
                else
                {
                    rollover = false;
                }

                nextTargetHour = path[currentPathIndex].z;
                currentTarget = new Vector2(path[currentPathIndex].x, path[currentPathIndex].y);
            }
        }
    }

    // Checks if hours reset and if so resets rollover
    void CheckRollover()
    {
        if (TimeSystem.gameTime.TotalHours < lastGameTime)
        {
            rollover = false;
        }

        lastGameTime = (float)TimeSystem.gameTime.TotalHours;
    }

    void MoveToTarget(Vector2 target)
    {
        lastMoveTime = Time.time;

        Vector2 down = new Vector2(0, -1);
        Vector2 left = new Vector2(-1, 0);
        Vector2 up = new Vector2(0, 1);
        Vector2 right = new Vector2(1, 0);

        // If on more similar vertical axis to target
        if (Mathf.Abs(transform.position.x - target.x) <= Mathf.Abs(transform.position.y - target.y))
        {
            // If below target
            if (transform.position.y < target.y)
            {
                // Try moving up
                if (!obstructed(up))
                {
                    Move(up);
                    return;
                }

                // If left of target
                if (transform.position.x <= target.x)
                {
                    // Try moving right
                    if (!obstructed(right))
                    {
                        Move(right);
                        return;
                    }

                    // Try moving left
                    if (!obstructed(left))
                    {
                        Move(left);
                        return;
                    }

                }
                // If right of target
                else
                {
                    // Try moving left
                    if (!obstructed(left))
                    {
                        Move(left);
                        return;
                    }

                    // Try moving right
                    if (!obstructed(right))
                    {
                        Move(right);
                        return;
                    }
                }

                return;
            }

            // If above target
            else
            {
                // Try moving down
                if (!obstructed(down))
                {
                    Move(down);
                    return;
                }

                // If left of target
                if (transform.position.x <= target.x)
                {
                    // Try moving right
                    if (!obstructed(right))
                    {
                        Move(right);
                        return;
                    }

                    if (!obstructed(left))
                    {
                        Move(left);
                        return;
                    }

                }
                // If right of target
                else
                {
                    // Try moving left
                    if (!obstructed(left))
                    {
                        Move(left);
                        return;
                    }

                    // Try moving right
                    if (!obstructed(right))
                    {
                        Move(right);
                        return;
                    }
                }

                return;
            }
        }
        else
        // If on more similar horizontal axis to target
        {
            // If left of target
            if (transform.position.x < target.x)
            {
                // Try moving right
                if (!obstructed(right))
                {
                    Move(right);
                    return;
                }

                // If below target
                if (transform.position.y <= target.y)
                {
                    // Try moving up
                    if (!obstructed(up))
                    {
                        Move(up);
                        return;
                    }

                    // Try moving down
                    if (!obstructed(down))
                    {
                        Move(down);
                        return;
                    }

                }
                // If above target
                else
                {
                    // Try moving down
                    if (!obstructed(down))
                    {
                        Move(down);
                        return;
                    }

                    // Try moving up
                    if (!obstructed(up))
                    {
                        Move(up);
                        return;
                    }
                }

                return;
            }
            // If right of target
            else
            {
                // Try moving left
                if (!obstructed(left))
                {
                    Move(left);
                    return;
                }

                // If below target
                if (transform.position.y <= target.y)
                {
                    // Try moving up
                    if (!obstructed(up))
                    {
                        Move(up);
                        return;
                    }

                    // Try moving down
                    if (!obstructed(down))
                    {
                        Move(down);
                        return;
                    }

                }
                // If above target
                else
                {
                    // Try moving down
                    if (!obstructed(down))
                    {
                        Move(down);
                        return;
                    }

                    // Try moving up
                    if (!obstructed(up))
                    {
                        Move(up);
                        return;
                    }
                }

                return;
            }
        }
    }

    // Returns whether position has collider obstructing it
    bool obstructed(Vector2 target)
    {
        Vector2 pos = (Vector2)transform.position + target;

        // Set check to size to just under full block in order to prevent collider bleeding
        Vector2 size = new Vector2(0.5f - errorMargin, 0.5f - errorMargin);

        // If non-trigger collider within the position found, return obstructed
        foreach (Collider2D collider in (Physics2D.OverlapBoxAll(pos, size, 0)))
        {
            if (!collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }

    void Move(Vector2 target)
    {
        rb.velocity = target;

        animator.SetFloat("Horizontal", target.x);
        animator.SetFloat("Vertical", target.y);
        animator.SetFloat("Speed", 1);
    }
}
