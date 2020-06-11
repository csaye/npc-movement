using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{

    public NPCScriptable NPCScriptable;

    void Start()
    {
        if (NPCScriptable != null)
        {
            LoadScriptable(NPCScriptable);
        }
    }

    void LoadScriptable(NPCScriptable NPCScriptable)
    {
        
    }
}
