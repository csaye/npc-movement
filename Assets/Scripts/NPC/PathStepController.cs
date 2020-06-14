using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathStepController : MonoBehaviour
{
    [Header("References: ")]
    public NPCScriptable NPCScriptable;
    
    private Rigidbody2D rb;
    private Animator animator;

    private List<Vector3> path;
    private int currentPathIndex;

    private List<Vector2> traveledPath = new List<Vector2>();

    private Vector2 currentTarget;
    private float nextTargetHour;

    private float lastMoveTime;

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
                if (Time.time - lastMoveTime >= 1)
                {
                    // If at target, stop
                    if (roundToHalf(transform.position.x) == currentTarget.x && roundToHalf(transform.position.y) == currentTarget.y)
                    {
                        rb.velocity = Vector2.zero;
                        animator.SetFloat("Speed", 0);

                        // Clear the traveled path
                        traveledPath.Clear();

                        // Snap to grid position for accuracy
                        transform.position = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));
                    }
                    else
                    // Move to target
                    {
                        MoveToTarget(currentTarget);

                        // Snap to grid position for accuracy
                        transform.position = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));
                    }
                }
            }
            else
            {
                // Clear the traveled path
                traveledPath.Clear();

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

    // Returns num rounded to the nearest half offset integer
    float roundToHalf(float num)
    {
        return (Mathf.Round(num + 0.5f) - 0.5f);
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

        Vector2 currentPos = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));

        // Add the current position to the traveled path and replace if already in list
        if (traveledPath.Contains(currentPos))
        {
            traveledPath.Remove(currentPos);
        }
        traveledPath.Add(currentPos);

        Vector2 down = new Vector2(currentPos.x, currentPos.y - 1);
        Vector2 left = new Vector2(currentPos.x - 1, currentPos.y);
        Vector2 up = new Vector2(currentPos.x, currentPos.y + 1);
        Vector2 right = new Vector2(currentPos.x + 1, currentPos.y);

        // If on more similar vertical axis to target
        if (Mathf.Abs(currentPos.x - target.x) <= Mathf.Abs(currentPos.y - target.y))
        {
            // If below target
            if (transform.position.y < target.y)
            {
                // If left of target
                if (transform.position.x < target.x)
                {
                    TryMove(up, right, left, down);
                }
                // If right of target
                else
                {
                    TryMove(up, left, right, down);
                }
            }
            // If above target
            else
            {
                // If left of target
                if (transform.position.x < target.x)
                {
                    TryMove(down, right, left, up);
                }
                // If right of target
                else
                {
                    TryMove(down, left, right, up);
                }
            }
        }
        // If on more similar horizontal axis to target
        else
        {
            // If left of target
            if (transform.position.x < target.x)
            {
                // If below target
                if (transform.position.y < target.y)
                {
                    TryMove(right, up, down, left);
                }
                // If above target
                else
                {
                    TryMove(right, down, up, left);
                }
            }
            // If right of target
            else
            {
                // If below target
                if (transform.position.y < target.y)
                {
                    TryMove(left, up, down, right);
                }
                // If above target
                else
                {
                    TryMove(left, down, up, right);
                }
            }
        }
    }

    void TryMove(Vector2 first, Vector2 second, Vector2 third, Vector2 fourth)
    {
        // Try first direction
        if (!obstructed(first) && !traveled(first))
        {
            Move(first);
            return;
        }
        
        // Try second direction
        if (!obstructed(second) && !traveled(second))
        {
            Move(second);
            return;
        }
        
        // Try third direction
        if (!obstructed(third) && !traveled(third))
        {
            Move(third);
            return;
        }
        
        // Try fourth direction
        if (!obstructed(fourth) && !traveled(fourth))
        {
            Move(fourth);
            return;
        }
        
        List<int> indexList = new List<int>();

        if (!obstructed(first)) {
            int firstIndex = traveledPath.IndexOf(first);
            indexList.Add(firstIndex);
        }
        if (!obstructed(second)) {
            int secondIndex = traveledPath.IndexOf(second);
            indexList.Add(secondIndex);
        }
        if (!obstructed(third)) {
            int thirdIndex = traveledPath.IndexOf(third);
            indexList.Add(thirdIndex);
        }
        if (!obstructed(fourth)) {
            int fourthIndex = traveledPath.IndexOf(fourth);
            indexList.Add(fourthIndex);
        }

        // If all walls are obstructed, return
        if (indexList.Count == 0)
        {
            return;
        }

        // Find the most recent traveled tile
        indexList.Sort();

        // Try first direction
        if (!obstructed(first) && (indexList[0] == traveledPath.IndexOf(first)))
        {
            Move(first);
            return;
        }
        
        // Try second direction
        if (!obstructed(second) && (indexList[0] == traveledPath.IndexOf(second)))
        {
            Move(second);
            return;
        }
        
        // Try third direction
        if (!obstructed(third) && (indexList[0] == traveledPath.IndexOf(third)))
        {
            Move(third);
            return;
        }
        
        // Try fourth direction
        if (!obstructed(fourth) && (indexList[0] == traveledPath.IndexOf(fourth)))
        {
            Move(fourth);
            return;
        }
    }

    // Returns whether position has collider obstructing it
    bool obstructed(Vector2 target)
    {
        // Set check to size to just under full block in order to prevent collider bleeding
        Vector2 size = new Vector2(0.499f, 0.499f);

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

    bool traveled(Vector2 target)
    {
        return (traveledPath.Contains(target));
    }

    void Move(Vector2 target)
    {
        Vector2 currentPos = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));

        Vector2 direction = target - currentPos;

        rb.velocity = direction;

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", 1);
    }
}
