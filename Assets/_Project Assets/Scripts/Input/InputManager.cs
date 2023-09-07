using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [SerializeField] private InputActionAsset playerInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            enabled = false;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public InputAction GetInputAction(string inputAction)
    {
        return playerInput.FindAction(inputAction);
    }
}
