using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private InputActionAsset playerInput;
    public enum ControlScheme { KBM, Controller };

    protected override void Awake()
    {
        base.Awake();
    }

    public InputAction GetInputAction(string inputAction)
    {
        return playerInput.FindAction(inputAction);
    }
}
