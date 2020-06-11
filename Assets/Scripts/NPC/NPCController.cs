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
                    // TEMP
                    transform.position = new Vector2(currentTarget.x, currentTarget.y);
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
}
