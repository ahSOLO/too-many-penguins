using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private Canvas creditsCanvas;
    [SerializeField] private Selectable startingElement;

    public void PlayGame()
    {
        GameManager.Instance.LoadScene("Level");
    }

    public void QuitGame()
    {
        GameManager.Instance.Quit();
    }

    private void OnEnable()
    {
        startingElement.Select();
    }

    public void ToggleCredits(bool isEnabled)
    {
        creditsCanvas.gameObject.SetActive(isEnabled);
    }
}
