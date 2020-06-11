using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{

    public NPCScriptable NPCScriptable;

    private List<Vector3> path;
    private int currentPathIndex;

    private Vector2 currentTarget;
    private float nextTargetHour;

    private float lastMoveTime;
    private float speed = 1;

    void Start()
    {
        if (path == null)
        {
            path = NPCScriptable.path;
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
            if (TimeSystem.gameTime.TotalHours < nextTargetHour)
            {
                // If not at target
                if ((Vector2)transform.position != currentTarget)
                {
                    // Move to target
                    if (Time.time - lastMoveTime >= speed)
                    {
                        MoveToTarget(currentTarget);
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

                nextTargetHour = path[currentPathIndex].z;
                currentTarget = new Vector2(path[currentPathIndex].x, path[currentPathIndex].y);
            }
        }
    }

    void MoveToTarget(Vector2 target)
    {
        lastMoveTime = Time.time;

        Vector2 down = new Vector2(transform.position.x, transform.position.y - 1);
        Vector2 left = new Vector2(transform.position.x - 1, transform.position.y);
        Vector2 up = new Vector2(transform.position.x, transform.position.y + 1);
        Vector2 right = new Vector2(transform.position.x + 1, transform.position.y);

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
                }

                return;
            }
        }
    }

    // Returns whether position has collider obstructing it
    bool obstructed(Vector2 target)
    {
        // Set check to size to just under full block in order to prevent collider bleeding
        Vector2 size = new Vector2(0.495f, 0.495f);

        // If non-trigger collider within the position found, return obstructed
        foreach (Collider2D collider in (Physics2D.OverlapBoxAll(target, size, 0)))
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
        transform.position = target;
    }
}
