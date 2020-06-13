using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    public static bool paused;

    public InputSystem inputSystem;

    void Awake()
    {
        inputSystem = new InputSystem();
        inputSystem.Menu.Escape.performed += ctx => EscPressed();
    }

    void EscPressed()
    {
        Debug.Log("the escape key was pressed");
    }

    private void OnEnable()
    {
        inputSystem.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Disable();
    }
}
