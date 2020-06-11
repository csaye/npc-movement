using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCScriptable", menuName = "Scriptables/NPC Scriptable")]
public class NPCScriptable : ScriptableObject
{
    public string NPCName;
    public List<Vector2> path;
}
