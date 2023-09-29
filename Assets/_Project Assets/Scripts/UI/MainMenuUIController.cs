using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private Canvas creditsCanvas;

    public void PlayGame()
    {
        GameManager.Instance.LoadScene("Level");
    }

    public void QuitGame()
    {
        GameManager.Instance.Quit();
    }

    public void ToggleCredits(bool isEnabled)
    {
        creditsCanvas.gameObject.SetActive(isEnabled);
    }
}
