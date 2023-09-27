using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{
    [SerializeField] private InputActionAsset mainInput;
    public enum ControlScheme { KBM, Controller };

    protected override void Awake()
    {
        base.Awake();
    }

    public InputAction GetInputAction(string inputAction)
    {
        return mainInput.FindAction(inputAction);
    }

    public InputActionMap GetActionMap(string actionMapName)
    {
        return mainInput.FindActionMap(actionMapName);
    }

    public void TogglePlayerInput(bool isActive)
    {
        if (isActive)
        {
            mainInput.FindActionMap("PlayerInput").Enable();
        }
        else
        {
            mainInput.FindActionMap("PlayerInput").Disable();
        }
    }
}
