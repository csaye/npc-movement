using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalStepController : MonoBehaviour
{
    [Header("References: ")]
    public NPCScriptable NPCScriptable;
    public GameObject testPrefab;
    
    private Rigidbody2D rb;
    private Animator animator;

    private List<Vector3> path;
    private int currentPathIndex;

    private List<Vector2> traveledPath = new List<Vector2>();

    private Vector2 currentTarget;
    private float nextTargetHour;

    private float lastMoveTime;

    // private bool rollover = false;

    private float lastGameTime;

    private float maximumIterations = 1000;

    private Vector2 roughPointer;
    private Vector2 pointer;

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

        // DEBUG
        foreach (Vector2 v2 in calculatePathTo(new Vector2(-2.5f, 3.5f)))
        {
            Instantiate(testPrefab, v2, Quaternion.identity);
        }
        // END DEBUG
    }

    void Update()
    {
        // if (!PauseSystem.paused)
        // {
        //     CheckRollover();
            
        //     // If next target hour not reached
        //     if ((!rollover && (TimeSystem.gameTime.TotalHours < nextTargetHour)) || (rollover && (TimeSystem.gameTime.TotalHours - 24 < nextTargetHour)))
        //     {
        //         if (Time.time - lastMoveTime >= 1)
        //         {
        //             // If at target, stop
        //             if (roundToHalf(transform.position.x) == currentTarget.x && roundToHalf(transform.position.y) == currentTarget.y)
        //             {
        //                 rb.velocity = Vector2.zero;
        //                 animator.SetFloat("Speed", 0);

        //                 // Clear the traveled path
        //                 traveledPath.Clear();

        //                 // Snap to grid position for accuracy
        //                 transform.position = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));
        //             }
        //             else
        //             // Move to target
        //             {
        //                 MoveToTarget(currentTarget);

        //                 // Snap to grid position for accuracy
        //                 transform.position = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));
        //             }
        //         }
        //     }
        //     else
        //     {
        //         // Clear the traveled path
        //         traveledPath.Clear();

        //         // Find the next target position and time
        //         if (currentPathIndex == path.Count - 1)
        //         {
        //             currentPathIndex = 0;
        //         }
        //         else
        //         {
        //             currentPathIndex++;
        //         }
                
        //         // Set to rollover if new target hour is less than current target hour
        //         if (path[currentPathIndex].z < nextTargetHour)
        //         {
        //             rollover = true;
        //         }
        //         else
        //         {
        //             rollover = false;
        //         }

        //         nextTargetHour = path[currentPathIndex].z;
        //         currentTarget = new Vector2(path[currentPathIndex].x, path[currentPathIndex].y);
        //     }
        // }
    }

    // Returns num rounded to the nearest half offset integer
    float roundToHalf(float num)
    {
        return (Mathf.Round(num + 0.5f) - 0.5f);
    }

    // // Checks if hours reset and if so resets rollover
    // void CheckRollover()
    // {
    //     if (TimeSystem.gameTime.TotalHours < lastGameTime)
    //     {
    //         rollover = false;
    //     }

    //     lastGameTime = (float)TimeSystem.gameTime.TotalHours;
    // }

    // void MoveToTarget(Vector2 target)
    // {
    //     lastMoveTime = Time.time;
    // }

    void Move(Vector2 target)
    {
        Vector2 currentPos = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));

        Vector2 direction = target - currentPos;

        rb.velocity = direction;

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", 1);
    }

    // Returns the most efficient path from the current player position to the target position
    List<Vector2> calculatePathTo(Vector2 target)
    {
        List<Vector2> roughPath = calculateRoughPathTo(target);
        List<Vector2> path = new List<Vector2>();

        Vector2 playerPos = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));
        pointer = target;

        traveledPath.Clear();

        // Cycle through points in rough path backwards until pointer is back at player position
        while (pointer != playerPos)
        {
            path.Add(pointer);

            // Add the current pointer position to the traveled path and replace if already in list
            if (traveledPath.Contains(pointer))
            {
                traveledPath.Remove(pointer);
            }
            traveledPath.Add(pointer);

            Vector2 down = new Vector2(pointer.x, pointer.y - 1);
            Vector2 left = new Vector2(pointer.x - 1, pointer.y);
            Vector2 up = new Vector2(pointer.x, pointer.y + 1);
            Vector2 right = new Vector2(pointer.x + 1, pointer.y);

            // If on more similar vertical axis to target
            if (Mathf.Abs(pointer.x - playerPos.x) <= Mathf.Abs(pointer.y - playerPos.y))
            {
                // If below target
                if (pointer.y < playerPos.y)
                {
                    // If left of target
                    if (pointer.x < playerPos.x)
                    {
                        TryMovePointer(roughPath, up, right, left, down);
                    }
                    // If right of target
                    else
                    {
                        TryMovePointer(roughPath, up, left, right, down);
                    }
                }
                // If above target
                else
                {
                    // If left of target
                    if (pointer.x < playerPos.x)
                    {
                        TryMovePointer(roughPath, down, right, left, up);
                    }
                    // If right of target
                    else
                    {
                        TryMovePointer(roughPath, down, left, right, up);
                    }
                }
            }
            // If on more similar horizontal axis to target
            else
            {
                // If left of target
                if (pointer.x < playerPos.x)
                {
                    // If below target
                    if (pointer.y < playerPos.y)
                    {
                        TryMovePointer(roughPath, right, up, down, left);
                    }
                    // If above target
                    else
                    {
                        TryMovePointer(roughPath, right, down, up, left);
                    }
                }
                // If right of target
                else
                {
                    // If below target
                    if (pointer.y < playerPos.y)
                    {
                        TryMovePointer(roughPath, left, up, down, right);
                    }
                    // If above target
                    else
                    {
                        TryMovePointer(roughPath, left, down, up, right);
                    }
                }
            }
        }

        // Clean up the path and reverse it
        path = cleanUpPath(path);
        path.Reverse();

        return path;
    }

    void TryMovePointer(List<Vector2> roughPath, Vector2 first, Vector2 second, Vector2 third, Vector2 fourth)
    {
        if (roughPath.Contains(first) && !traveled(first))
        {
            pointer = first;
            return;
        }
        if (roughPath.Contains(second) && !traveled(second))
        {
            pointer = second;
            return;
        }
        if (roughPath.Contains(third) && !traveled(third))
        {
            pointer = third;
            return;
        }
        if (roughPath.Contains(fourth) && !traveled(fourth))
        {
            pointer = fourth;
            return;
        }
        if (roughPath.Contains(first))
        {
            pointer = first;
            return;
        }
        if (roughPath.Contains(second))
        {
            pointer = second;
            return;
        }
        if (roughPath.Contains(third))
        {
            pointer = third;
            return;
        }
        if (roughPath.Contains(fourth))
        {
            pointer = fourth;
            return;
        }
    }

    // Cleans up all instances between two identical 
    List<Vector2> cleanUpPath(List<Vector2> path)
    {
        List<Vector2> cleanPath = path;

        foreach(Vector2 v2 in cleanPath)
        {
            // If element appears more than once in the array
            if (repeatedInList(v2, cleanPath))
            {
                // Cut out all elements between instances of the element
                try
                {
                    cleanPath.RemoveRange(cleanPath.IndexOf(v2), cleanPath.LastIndexOf(v2) - 1);
                }
                catch
                {
                    Debug.Log("error lol");
                }
            }
        }

        return cleanPath;
    }

    bool repeatedInList(Vector2 v2, List<Vector2> list)
    {
        int instances = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (v2 == list[i])
            {
                instances++;
                // If 2 or more instances in list, return true
                if (instances >= 2)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Returns a rough path from the current player position to the target position
    List<Vector2> calculateRoughPathTo(Vector2 target)
    {
        traveledPath.Clear();

        List<Vector2> roughPath = new List<Vector2>();

        Vector2 playerPos = new Vector2(roundToHalf(transform.position.x), roundToHalf(transform.position.y));
        roughPointer = playerPos;

        // From player position to target
        for (int i = 0; i < maximumIterations; i++)
        {
            roughPath.Add(roughPointer);

            if (roughPointer == target)
            {
                traveledPath.Clear();
                // break;
                return roughPath;
            }

            // Add the current pointer position to the traveled path and replace if already in list
            if (traveledPath.Contains(roughPointer))
            {
                traveledPath.Remove(roughPointer);
            }
            traveledPath.Add(roughPointer);

            Vector2 down = new Vector2(roughPointer.x, roughPointer.y - 1);
            Vector2 left = new Vector2(roughPointer.x - 1, roughPointer.y);
            Vector2 up = new Vector2(roughPointer.x, roughPointer.y + 1);
            Vector2 right = new Vector2(roughPointer.x + 1, roughPointer.y);

            // If on more similar vertical axis to target
            if (Mathf.Abs(roughPointer.x - target.x) <= Mathf.Abs(roughPointer.y - target.y))
            {
                // If below target
                if (roughPointer.y < target.y)
                {
                    // If left of target
                    if (roughPointer.x < target.x)
                    {
                        TryMoveRoughPointer(up, right, left, down);
                    }
                    // If right of target
                    else
                    {
                        TryMoveRoughPointer(up, left, right, down);
                    }
                }
                // If above target
                else
                {
                    // If left of target
                    if (roughPointer.x < target.x)
                    {
                        TryMoveRoughPointer(down, right, left, up);
                    }
                    // If right of target
                    else
                    {
                        TryMoveRoughPointer(down, left, right, up);
                    }
                }
            }
            // If on more similar horizontal axis to target
            else
            {
                // If left of target
                if (roughPointer.x < target.x)
                {
                    // If below target
                    if (roughPointer.y < target.y)
                    {
                        TryMoveRoughPointer(right, up, down, left);
                    }
                    // If above target
                    else
                    {
                        TryMoveRoughPointer(right, down, up, left);
                    }
                }
                // If right of target
                else
                {
                    // If below target
                    if (roughPointer.y < target.y)
                    {
                        TryMoveRoughPointer(left, up, down, right);
                    }
                    // If above target
                    else
                    {
                        TryMoveRoughPointer(left, down, up, right);
                    }
                }
            }
        }

        // // From target to player position
        // for (int i = 0; i < maximumIterations; i++)
        // {
        //     roughPath.Add(roughPointer);

        //     if (roughPointer == playerPos)
        //     {
        //         traveledPath.Clear();
        //         return roughPath;
        //     }

        //     // Add the current pointer position to the traveled path and replace if already in list
        //     if (traveledPath.Contains(roughPointer))
        //     {
        //         traveledPath.Remove(roughPointer);
        //     }
        //     traveledPath.Add(roughPointer);

        //     Vector2 down = new Vector2(roughPointer.x, roughPointer.y - 1);
        //     Vector2 left = new Vector2(roughPointer.x - 1, roughPointer.y);
        //     Vector2 up = new Vector2(roughPointer.x, roughPointer.y + 1);
        //     Vector2 right = new Vector2(roughPointer.x + 1, roughPointer.y);

        //     // If on more similar vertical axis to target
        //     if (Mathf.Abs(roughPointer.x - playerPos.x) <= Mathf.Abs(roughPointer.y - playerPos.y))
        //     {
        //         // If below target
        //         if (roughPointer.y < playerPos.y)
        //         {
        //             // If left of target
        //             if (roughPointer.x < playerPos.x)
        //             {
        //                 TryMoveRoughPointer(up, right, left, down);
        //             }
        //             // If right of target
        //             else
        //             {
        //                 TryMoveRoughPointer(up, left, right, down);
        //             }
        //         }
        //         // If above target
        //         else
        //         {
        //             // If left of target
        //             if (roughPointer.x < playerPos.x)
        //             {
        //                 TryMoveRoughPointer(down, right, left, up);
        //             }
        //             // If right of target
        //             else
        //             {
        //                 TryMoveRoughPointer(down, left, right, up);
        //             }
        //         }
        //     }
        //     // If on more similar horizontal axis to target
        //     else
        //     {
        //         // If left of target
        //         if (roughPointer.x < playerPos.x)
        //         {
        //             // If below target
        //             if (roughPointer.y < playerPos.y)
        //             {
        //                 TryMoveRoughPointer(right, up, down, left);
        //             }
        //             // If above target
        //             else
        //             {
        //                 TryMoveRoughPointer(right, down, up, left);
        //             }
        //         }
        //         // If right of target
        //         else
        //         {
        //             // If below target
        //             if (roughPointer.y < playerPos.y)
        //             {
        //                 TryMoveRoughPointer(left, up, down, right);
        //             }
        //             // If above target
        //             else
        //             {
        //                 TryMoveRoughPointer(left, down, up, right);
        //             }
        //         }
        //     }
        // }

        return null;
    }

    void TryMoveRoughPointer(Vector2 first, Vector2 second, Vector2 third, Vector2 fourth)
    {
        // Try first direction
        if (!obstructed(first) && !traveled(first))
        {
            MoveRoughPointer(first);
            return;
        }
        
        // Try second direction
        if (!obstructed(second) && !traveled(second))
        {
            MoveRoughPointer(second);
            return;
        }
        
        // Try third direction
        if (!obstructed(third) && !traveled(third))
        {
            MoveRoughPointer(third);
            return;
        }
        
        // Try fourth direction
        if (!obstructed(fourth) && !traveled(fourth))
        {
            MoveRoughPointer(fourth);
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
            MoveRoughPointer(first);
            return;
        }
        
        // Try second direction
        if (!obstructed(second) && (indexList[0] == traveledPath.IndexOf(second)))
        {
            MoveRoughPointer(second);
            return;
        }
        
        // Try third direction
        if (!obstructed(third) && (indexList[0] == traveledPath.IndexOf(third)))
        {
            MoveRoughPointer(third);
            return;
        }
        
        // Try fourth direction
        if (!obstructed(fourth) && (indexList[0] == traveledPath.IndexOf(fourth)))
        {
            MoveRoughPointer(fourth);
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

    void MoveRoughPointer(Vector2 target)
    {
        roughPointer = target;
    }
}
