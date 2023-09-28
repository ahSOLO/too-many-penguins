using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private BoolEvent pauseGameToggle;
    public bool gameIsPausable;
    private InputAction pauseAction;
    private bool gameIsPaused;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InputManager.Instance.GetActionMap("GameInput").Enable();

        pauseAction = InputManager.Instance.GetInputAction("Toggle Pause");
        pauseAction.performed += ctx => OnPauseAction();

        pauseGameToggle.Register(OnPauseGameToggle);
    }

    private void OnPauseAction()
    {
        if (gameIsPausable)
        {
            pauseGameToggle.Raise(!gameIsPaused);        
        }
    }

    private void OnPauseGameToggle(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0;
            InputManager.Instance.TogglePlayerInput(false);
        }
        else
        {
            Time.timeScale = 1;
            InputManager.Instance.TogglePlayerInput(true);
        }
        gameIsPaused = isPaused;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
